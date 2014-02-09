using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMap
{
    public struct Element
    {
        public bool bIsWater;
        public int iElevation;
        public int iPopulation;
        public int iNation;
        public int x;
        public int y;
        public int iMaxPopulation;

    }

    public struct Nation
    {
        public int iExcessPopulation;
        public int id { get; set; }
        public int iPopulation { get; set; }
        public List<Element> lAreas;
    }

    public struct Board
    {
        public int xSize;
        public int ySize;
        public int PctWater;
        public Element[,] Map;

        public int SeaLevel;
        public int MinElevation;
        public int MaxElevation;
        public int AvgElevation;
        public int iBelowSealevel;
        public int iNationCnt;
        public List<Nation> lNations;


    }

    public class Map 
    {
        private Board _Board;
        private Random rnd = new Random();

        #region Initialization
        private Map()
        {
            _Board.xSize = 50;
            _Board.ySize = 50;
            _Board.PctWater = 50;
            _Board.SeaLevel = 0;
            _Board.iNationCnt = 0;
            InitMap();
        }
        public Map(int xSize, int ySize, int iPctWater)
        {
            _Board.xSize = xSize;
            _Board.ySize = ySize;
            _Board.PctWater = iPctWater;
            InitMap();
        }

        private void InitElement (int x, int y)
        {
            
            _Board.Map[x, y].iNation = -1;
            _Board.Map[x, y].iPopulation = 0;
            _Board.Map[x, y].iElevation = 1 * (rnd.Next() % 100);
            _Board.Map[x, y].x = x;
            _Board.Map[x, y].y = y;

        }

        private void InitMap()
        {
            _Board.Map = new Element[_Board.xSize, _Board.ySize];
            _Board.lNations = new List<Nation>();
            for (int x = 0; x < _Board.xSize; x++)
            {
                for (int y = 0; y < _Board.ySize; y++)
                {
                    InitElement(x, y);
                }
            }
            SetSeaLevel();

        }
        public void SetSeaLevel()
        {
            int[] elevations = new int[_Board.xSize * _Board.ySize];

            for (int x = 0; x < _Board.xSize; x++)
            {

                for (int y = 0; y < _Board.ySize; y++)
                {
                    elevations[y * _Board.xSize + x] = _Board.Map[x, y].iElevation;
                }

            }
            Array.Sort(elevations);

            int iPrcInd = _Board.xSize * _Board.ySize * _Board.PctWater / 100;
            _Board.SeaLevel = elevations[iPrcInd];
            _Board.MaxElevation = elevations.Max();
            _Board.MinElevation = elevations.Min();
            _Board.AvgElevation = (int)elevations.Average();
            _Board.iBelowSealevel = 0;
            for (int x = 0; x < _Board.xSize; x++)
            {

                for (int y = 0; y < _Board.ySize; y++)
                {
                    _Board.Map[x, y].bIsWater = (_Board.Map[x, y].iElevation <= _Board.SeaLevel);
                    if (_Board.Map[x, y].bIsWater)
                        _Board.iBelowSealevel++;
                }
            }

        }
        #endregion

        #region Land Grouping
        private int calcWeight(int x, int y)
        {
            int iCluster = 1;
            int sum = 0;
            int cnt = 0;
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1 * iCluster; j <= iCluster; j++)
                {
                    int xTest = x + i;
                    int yTest = y + j;

                    if ((x + i) >= _Board.xSize)
                        xTest = 0;

                    if ((x + i) < 0)
                        xTest = _Board.xSize - 1;

                    if ((y + j) >= _Board.ySize)
                        yTest = 0;

                    if ((y + j) < 0)
                        yTest = _Board.ySize - 1;

                    if ((xTest < _Board.xSize) && (yTest < _Board.ySize)
                        && (xTest >= 0) && (yTest >= 0)

                        )
                    {

                        sum += _Board.Map[xTest, yTest].iElevation;
                        cnt++;
                    }
                    else
                    {
                        int a = 0; // exception
                    }
                }
            }

            return (int)(sum / cnt);

        }
        public void Group()
        {
            
            // Smoothing
            for (int k = 0; k < 1000000; k++)
            {
                int x1 = rnd.Next() % _Board.xSize;
                int y1 = rnd.Next() % _Board.ySize;
                int w1 = calcWeight(x1, y1);
                int a1 = _Board.Map[x1, y1].iElevation;
                int diff1 = Math.Abs(a1 - w1);

                int x2 = rnd.Next() % _Board.xSize;
                int y2 = rnd.Next() % _Board.ySize;
                int w2 = calcWeight(x2, y2);
                int a2 = _Board.Map[x2, y2].iElevation;
                int diff2 = Math.Abs(a2 - w2);

                int diff3 = Math.Abs(a1 - w2);
                int diff4 = Math.Abs(a2 - w1);

                if ((diff3 + diff4) < (diff1 + diff2))
                {
                    _Board.Map[x2, y2].iElevation = a1;
                    _Board.Map[x1, y1].iElevation = a2;
                }

            }
            SetSeaLevel();

        }
        #endregion

        public Board GetBoard()
        {
            return _Board;
        }

        public int AddNation()
        {
            for (int i = 0; i < 100; i++)
            {
                int x = rnd.Next() % _Board.xSize;
                int y = rnd.Next() % _Board.ySize;

                if (_Board.Map[x, y].bIsWater == false)
                {
                    _Board.Map[x, y].iNation = _Board.iNationCnt++;
                    _Board.Map[x, y].iPopulation = 100;
                    Nation item = new Nation();
                    item.id = _Board.Map[x, y].iNation; 
                    item.iPopulation = 100;
                    item.lAreas = new List<Element>();
                    item.lAreas.Add (_Board.Map[x, y]);
                    _Board.lNations.Add(item);
                    return _Board.Map[x, y].iNation;
                }
            }
            return -1;
        }

        public int ExpandInfluence ()
        {
            // For each region each nation controls
            // expand influence to regions where population is present
            // expand visablity to regions adjacent to where influence is present

            foreach (Nation item in _Board.lNations)
            {
                List<Element> lTemp = new List<Element>();
                    
                foreach (Element region in item.lAreas)
                {
                    if (region.iPopulation == 0)
                        continue;
                    // test surounding regions
                    for (int i = -1; i <=1; i++)
                    {
                        for (int j = -1; j <=1; j++)
                        {
                            int x = region.x;
                            int y = region.y;

                            if (_Board.Map[x + i, y + j].iNation == -1)
                            {
                                _Board.Map[x + i, y + j].iNation = region.iNation;
                                lTemp.Add(_Board.Map[x + i, y + j]);
                            }
                        }
                    }
                    
                }
                item.lAreas.AddRange(lTemp);

            }
            return 1;
        }

        public int IncreasePopulation()
        {
            // For each region each nation controls
            // expand population to regions where population is present

            foreach (Nation item in _Board.lNations)
            {
                List<Element> lTemp = new List<Element>();

                foreach (Element region in item.lAreas)
                {
                    if (region.iPopulation == 0)
                        continue;

                    int iNewPop = (int)(region.iPopulation * 1.5); // birthrate tb calc'd based on attributes
                    int iExcess = Math.Max(iNewPop - region.iMaxPopulation, 0); // excess
                    iNewPop = Math.Min(iNewPop, region.iMaxPopulation); // new pop

                }
                item.lAreas.AddRange(lTemp);

            }
            return 1;
        }
    }

    public class MapInter
    {
        private static Map _Map = null;
       
        public static bool InitBoard(int xSize, int ySize, int iPctWater = 50)
        {
            if (_Map == null)
            {
                _Map = new Map(xSize, ySize, iPctWater);
            }
            return true;
        }
        

        static public void Group()
        {
            if (_Map != null)
            {
                _Map.Group();
            }

        }

        public static Board GetBoard()
        {
            return _Map.GetBoard();
            
        }

        public static int AddNation ()
        {
            return _Map.AddNation();

        }

        public static int ExpandInfluence()
        {

            return _Map.ExpandInfluence();
        }

        public static int SimulateTurn ()
        {
            // Go through each square
            // Increase population test...
            // Migrate population

            return 1;
        }
    }
}
