using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HRMap;
namespace HistoryRewind
{
    public partial class Form1 : Form
    {
        private void ShowBoard ()
        {
            Board board = MapInter.GetBoard();
            Element[,] map = board.Map;
            int xSize = map.GetLength(0);
            int ySize = map.GetLength(1);

            int iMx = 16;

            Bitmap bmp = new Bitmap(xSize * iMx, ySize * iMx);
            for (int x = 0; x < xSize; x++)
            {
                for (int y = 0; y < ySize; y++)
                {
                    Color col = Color.SaddleBrown;
                    if (map[x, y].bIsWater)
                        col = Color.Blue;
                    else if (map[x,y].iNation >= 0)
                    {
                        col = Color.Green;
                    }

                 //   col = Color.FromArgb(map[x, y].iElevation * 2
                   //         , map[x, y].iElevation * 2, map[x, y].iElevation * 2);

                    for (int i = 0; i < iMx; i++)
                        for (int j = 0; j < iMx; j++)
                            bmp.SetPixel(x * iMx + i, y * iMx + j, col);
                }
            }

            for (int x = 0; x < xSize*iMx; x++)
            {
                for (int y = 0; y < ySize*iMx; y++)
                {
                    if ((x%16 == 0) || (y%16==0))
                    bmp.SetPixel(x, y, Color.Black);
                }
            }
            pbMap.Image = bmp;

            label1.Text = "Sealevel " + board.SeaLevel + " ft";

            label2.Text = "Min Elev " + board.MinElevation + " ft";
            label3.Text = "Max Elev " + board.MaxElevation + " ft";
            label4.Text = "Avg Elev " + board.AvgElevation + " ft";
            label5.Text = "Water Sqrs " + board.iBelowSealevel;
            label6.Text = "Total Sqrs " + board.xSize *board.ySize ;
            label7.Text = "Nations " + board.iNationCnt;

            if (board.lNations.Count > 0)
            {
                BindingSource source = new BindingSource();
                source.DataSource = board.lNations;
                dataGridView1.DataSource = source;
               // dataGridView1.DataSource = board.lNations;
               //               select new { Column1 = arr.id, Column2 = arr.iPopulation });

             //   dataGridView1.Columns.Add("a", "a");
              //  dataGridView1.Columns.Add("b", "b");
              //  dataGridView1.Columns[0].DataPropertyName = "Column1";
               // dataGridView1.Columns[1].DataPropertyName = "Column2";
            }
        }
        public Form1()
        {
            InitializeComponent();
            MapInter.InitBoard(100, 100, 60);
            ShowBoard();
      
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MapInter.Group();
            ShowBoard();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MapInter.AddNation();
            ShowBoard();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            MapInter.ExpandInfluence();
            ShowBoard();
        }
    }
}
