﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Bunifu.Framework.UI;

namespace PaintUI
{
    public partial class Form1 : Form
    {
        //Khai bao bien
        enum Tools {BRUSH, SHAPE, FILLBUCKET, ERASER };

        Tools curTool;
        Pen pen;
        Bitmap bm;
        Graphics gra;
        Bitmap temp;
        Point old, cur;
        SolidBrush eraser,fillColor;

        bool isDown;
        int wid, hei;
        int penSize;

        public Form1()
        {
            InitializeComponent();
          
            HideAllPanel();
            brushesPanel.Show();

            penSize = 10;
            pen = new Pen(Color.Black, penSize);
            bm = new Bitmap(SketchBox.Width, SketchBox.Height, SketchBox.CreateGraphics());
            gra = Graphics.FromImage(bm);

            isDown = false;
            curTool = Tools.BRUSH;

            menuPanel.BringToFront();
            SketchBox.Cursor = Cursors.Cross;
            
            //Modify stroke
            pen.SetLineCap(System.Drawing.Drawing2D.LineCap.Round, System.Drawing.Drawing2D.LineCap.Round, System.Drawing.Drawing2D.DashCap.Round);


            //Smoothing
            {
                this.SetStyle(ControlStyles.UserPaint, true);
                this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
                this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);

                gra.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                gra.Clear(Color.White);
            }

            menuPanel.NewButtonClick += MenuPanel_NewButtonClick;
            menuPanel.OpenButtonClick += MenuPanel_OpenButtonClick;
            menuPanel.SaveButtonClick += MenuPanel_SaveButtonClick;
            menuPanel.SaveAsButtonClick += MenuPanel_SaveAsButtonClick;

            LeftTopPanel.Location = new Point(SketchBox.Location.X - 22, SketchBox.Location.Y - 22);
            RightBottomPanel.Location = new Point(SketchBox.Location.X + SketchBox.Width, SketchBox.Location.Y + SketchBox.Height);
            RightTopPanel.Location = new Point(SketchBox.Location.X + SketchBox.Width, SketchBox.Location.Y-22);
            LeftBottomPanel.Location = new Point(SketchBox.Location.X-22, SketchBox.Location.Y + SketchBox.Height);

            
        }



        //Xu li cac xu kien cua menuPanel

        private void MenuPanel_SaveAsButtonClick(object sender, EventArgs e)
        {
            throw new NotImplementedException();
            
        }

        private void MenuPanel_SaveButtonClick(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Bmp (*.bmp)|*.bmp|Jpg (*.jpg)|*.jpg|Jpeg (*.jpeg)|*.jpeg|Png (*.png)|*.png";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                bm.Save(sfd.FileName);
            }
        }

        private void MenuPanel_OpenButtonClick(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Image files(*.jpg,*.jpeg, *.png, *.bmp)|*jpg; *jpeg; **png; *bmp";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Image img = Image.FromFile(ofd.FileName);
                bm = new Bitmap(SketchBox.Width, SketchBox.Height);
                gra = Graphics.FromImage(bm);
                gra.DrawImage(img, new Rectangle(0, 0, bm.Width, bm.Height));
                SketchBox.Refresh();
                SketchBox.BackgroundImage = (Bitmap)bm.Clone();
            }
        }

        private void MenuPanel_NewButtonClick(object sender, EventArgs e)
        {
            bm = new Bitmap(this.Width, this.Height);
            gra = Graphics.FromImage(bm);
            SketchBox.Refresh();
            SketchBox.BackgroundImage = (Bitmap)bm.Clone();
        }



        //Giau Panels
        private void HideAllPanel()
        {
            shapesPanel.Visible = false;
            textPanel.Visible = false;
            canvasPanel.Visible = false;
            brushesPanel.Visible = false;
            effectsPanel.Visible = false;
            menuPanel.Visible = false;
            LeftTopPanel.Visible = false;
            LeftBottomPanel.Visible = false;
            RightTopPanel.Visible = false;
            RightBottomPanel.Visible = false;
        }
        
        
        
        //Code cac chuc nang cho cac WindowState Butttons
        private void MinimizeButton_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
        private void MaximizeButton_Click(object sender, EventArgs e)
        {
            if (this.WindowState != FormWindowState.Maximized)
                this.WindowState = FormWindowState.Maximized;
            else this.WindowState = FormWindowState.Normal;
        }
        private void CloseButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        //Hien thi cac Panels khi click va hover va leave
       

        private void MenuButton_Click(object sender, EventArgs e)
        {
            bunifuTransition1.ShowSync(menuPanel, false, BunifuAnimatorNS.Animation.HorizSlide);
        }

        private void TextButton_Click(object sender, EventArgs e)
        {
            if (!textPanel.Visible)
            {
                HideAllPanel();
                textPanel.Show();
            }
            
        }
        private void ShapesButton_Click(object sender, EventArgs e)
        {
            if (!shapesPanel.Visible)
            {
                HideAllPanel();
                shapesPanel.Show();
            }
            curTool = Tools.SHAPE;
        }
        private void CanvasButton_Click(object sender, EventArgs e)
        {
            if (!canvasPanel.Visible)
            {
                HideAllPanel();
                canvasPanel.Show();
                LeftTopPanel.Visible = true;
                LeftBottomPanel.Visible = true;
                RightTopPanel.Visible = true;
                RightBottomPanel.Visible = true;
            }
          
        }

        private void BrushesButton_Click(object sender, EventArgs e)
        {
            if (!brushesPanel.Visible)
            {
                HideAllPanel();
                brushesPanel.Show();
            }
            curTool = Tools.BRUSH;

            gra.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
        }
        private void EffectsButton_Click(object sender, EventArgs e)
        {
            if (!effectsPanel.Visible)
            {
                HideAllPanel();
                effectsPanel.Show();
            }
                
        }

        //Cac su kien voi mouse
        private void SketchBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDown)
            {
                cur = new Point(e.Location.X, e.Location.Y);
                wid = cur.X - old.X;
                hei = cur.Y - old.Y;
                SketchBox.Refresh();
            }
        }

        private void SketchBox_MouseUp(object sender, MouseEventArgs e)
        {
            isDown = false;
            if(curTool == Tools.SHAPE)
            {

                shapesPanel.DrawShapes(SketchBox, bm, gra, old, cur, new Size(wid, hei), pen, fillColor, true);
            }
            wid = hei = 0;
        }

        Color AdjustBrightness(Color c1, float factor)
        {

            float r = ((c1.R * factor) > 255) ? 255 : (c1.R * factor);
            float g = ((c1.G * factor) > 255) ? 255 : (c1.G * factor);
            float b = ((c1.B * factor) > 255) ? 255 : (c1.B * factor);

            Color c = Color.FromArgb((int)r, (int)g, (int)b);
            return c;

        }

        private void FillButton_Click(object sender, EventArgs e)
        {
            curTool = Tools.FILLBUCKET;
        }

        private void EraserButton_Click(object sender, EventArgs e)
        {
            curTool = Tools.ERASER;
        }

        private void SketchBox_MouseDown(object sender, MouseEventArgs e)
        {
            isDown = true;
            //Color c2 = Color.FromArgb((int)(colorPanel.getColor().A*0.5),(int)(colorPanel.getColor().R), (int)(colorPanel.getColor().G), (int)(colorPanel.getColor().B ));

            pen = new Pen(colorPanel.getColor1(), brushesPanel.getThickness());
            pen.SetLineCap(System.Drawing.Drawing2D.LineCap.Round, System.Drawing.Drawing2D.LineCap.Round, System.Drawing.Drawing2D.DashCap.Round);
            fillColor=eraser = new SolidBrush(colorPanel.getColor2());
            old = new Point(e.Location.X, e.Location.Y);
            cur = old;     
           
            if(curTool==Tools.FILLBUCKET)
            {
                FillBucket bucket = new FillBucket();
                bucket.Fill(bm, old, bm.GetPixel(old.X, old.Y), pen.Color);
                SketchBox.BackgroundImage = (Bitmap)bm.Clone();
            }
           
        }

        private void SketchBox_Paint(object sender, PaintEventArgs e)
        {
            
            if (isDown)
            {
                switch (curTool)
                {
                    case Tools.BRUSH:
                        gra.FillEllipse(pen.Brush, cur.X - brushesPanel.getThickness() / 2, cur.Y - brushesPanel.getThickness() / 2, brushesPanel.getThickness(), brushesPanel.getThickness());
                        gra.DrawLine(pen, old, cur);
                        old = cur;
                        SketchBox.BackgroundImage = (Bitmap)bm.Clone();
                        break;
                    case Tools.SHAPE:
                        shapesPanel.DrawShapes(SketchBox, bm, e.Graphics, old, cur, new Size(wid, hei), pen, fillColor, false);
                        break;
                    case Tools.ERASER:
                        gra.FillRectangle(eraser, cur.X - brushesPanel.getThickness() / 2, cur.Y - brushesPanel.getThickness() / 2, brushesPanel.getThickness(), brushesPanel.getThickness());
                        Pen temp = new Pen(eraser.Color, brushesPanel.getThickness());
                        temp.SetLineCap(System.Drawing.Drawing2D.LineCap.Square, System.Drawing.Drawing2D.LineCap.Square, System.Drawing.Drawing2D.DashCap.Round);
                        gra.DrawLine(temp, old, cur);
                        old = cur;
                        SketchBox.BackgroundImage = (Bitmap)bm.Clone();
                        break;
                    default:
                        break;
                }
            }
        }

        private void SketchBox_SizeChanged(object sender, EventArgs e)
        {
            LeftTopPanel.Location = new Point(SketchBox.Location.X - 22, SketchBox.Location.Y - 22);
            RightBottomPanel.Location = new Point(SketchBox.Location.X + SketchBox.Width, SketchBox.Location.Y + SketchBox.Height);
            RightTopPanel.Location = new Point(SketchBox.Location.X + SketchBox.Width, SketchBox.Location.Y-22);
            LeftBottomPanel.Location = new Point(SketchBox.Location.X-22, SketchBox.Location.Y + SketchBox.Height);
        }
        private void SketchBox_LocationChanged(object sender, EventArgs e)
        {
            LeftTopPanel.Location = new Point(SketchBox.Location.X - 22, SketchBox.Location.Y -22);
            RightBottomPanel.Location = new Point(SketchBox.Location.X + SketchBox.Width, SketchBox.Location.Y + SketchBox.Height);
            RightTopPanel.Location=new Point(SketchBox.Location.X + SketchBox.Width, SketchBox.Location.Y-22);
            LeftBottomPanel.Location = new Point(SketchBox.Location.X-22, SketchBox.Location.Y + SketchBox.Height);
        }
        //---------------
        //Resize SketchBox
        Point pOld, startPoint,oldLocation;
        bool isDragged = false; 
       
        private void LeftTopPanel_MouseDown(object sender, MouseEventArgs e)
        {
            startPoint = e.Location;
            temp = (Bitmap)bm;
            isDragged = true;
        }

        private void LeftTopPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragged)
            {

                pOld = e.Location;
                oldLocation = SketchBox.Location;
                if (LeftTopPanel.Location.X - startPoint.X + pOld.X < panelCavas.Width / 2 - 50
                    && LeftTopPanel.Location.Y - startPoint.Y + pOld.Y < panelCavas.Height / 2 - 50)
                {    
                    LeftTopPanel.Location = new Point(LeftTopPanel.Location.X - startPoint.X + pOld.X, LeftTopPanel.Location.Y - startPoint.Y + pOld.Y);
                    SketchBox.Location = new Point(oldLocation.X - startPoint.X + pOld.X, oldLocation.Y - startPoint.Y + pOld.Y);
                    SketchBox.Size = new Size(SketchBox.Width + startPoint.X - pOld.X, SketchBox.Height + startPoint.Y - pOld.Y);
                }
                bm = new Bitmap(SketchBox.Width, SketchBox.Height);
                gra = Graphics.FromImage(bm);
                gra.Clear(Color.White);
                gra.DrawImage(temp, 0, 0, temp.Width, temp.Height);
                SketchBox.BackgroundImage = (Bitmap)bm.Clone();
            }
        }

        private void LeftTopPanel_MouseUp(object sender, MouseEventArgs e)
        {
            isDragged = false;
        }

        private void LeftBottomPanel_MouseDown(object sender, MouseEventArgs e)
        {
            startPoint = e.Location;
            temp = (Bitmap)bm;
            isDragged = true;
        }
 
        private void LeftBottomPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragged)
            {

                pOld = e.Location;
                oldLocation = SketchBox.Location;
                if (LeftBottomPanel.Location.X - startPoint.X + pOld.X < panelCavas.Width/2 - 50
                  && LeftBottomPanel.Location.Y - startPoint.Y + pOld.Y > panelCavas.Height/2 + 50)
                {  
                    LeftBottomPanel.Location = new Point(LeftBottomPanel.Location.X - startPoint.X + pOld.X, LeftBottomPanel.Location.Y - startPoint.Y + pOld.Y);
                    SketchBox.Location = new Point(oldLocation.X - startPoint.X + pOld.X, oldLocation.Y);
                    SketchBox.Size = new Size(SketchBox.Width + startPoint.X - pOld.X,SketchBox.Height - startPoint.Y + pOld.Y);
                }
                bm = new Bitmap(SketchBox.Width, SketchBox.Height);
                gra = Graphics.FromImage(bm);
                gra.Clear(Color.White);
                gra.DrawImage(temp, 0, 0, temp.Width, temp.Height);
                SketchBox.BackgroundImage = (Bitmap)bm.Clone();
            }
        }
      
        private void LeftBottomPanel_MouseUp(object sender, MouseEventArgs e)
        {
                isDragged = false;
           
        }

        private void RightTopPanel_MouseDown(object sender, MouseEventArgs e)
        {
            startPoint = e.Location;
            temp = (Bitmap)bm;
            isDragged = true;
        }

        private void RightTopPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragged)
            {
                pOld = e.Location;
                oldLocation = SketchBox.Location;
                if (RightTopPanel.Location.X - startPoint.X + pOld.X > panelCavas.Width / 2 + 50
                   && RightTopPanel.Location.Y - startPoint.Y + pOld.Y < panelCavas.Height/2 - 50)
                {
                    RightTopPanel.Location = new Point(RightTopPanel.Location.X - startPoint.X + pOld.X, RightTopPanel.Location.Y - startPoint.Y + pOld.Y);
                    SketchBox.Location = new Point(oldLocation.X, oldLocation.Y - startPoint.Y + pOld.Y);
                    SketchBox.Size = new Size(SketchBox.Width - startPoint.X + pOld.X, SketchBox.Height + startPoint.Y - pOld.Y);
                }
                
                bm = new Bitmap(SketchBox.Width, SketchBox.Height);
                gra = Graphics.FromImage(bm);
                gra.Clear(Color.White);
                gra.DrawImage(temp, 0, 0, temp.Width, temp.Height);
                SketchBox.BackgroundImage = (Bitmap)bm.Clone();
            }
        }

        private void RightTopPanel_MouseUp(object sender, MouseEventArgs e)
        {
            isDragged = false;
        }

        private void RightBottomPanel_1_MouseDown(object sender, MouseEventArgs e)
        {
            startPoint = e.Location;
            temp = (Bitmap)bm;
            isDragged = true;
        }

        private void RightBottomPanel_1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragged)
            {
                pOld = e.Location;
                if (RightBottomPanel.Location.X - startPoint.X + pOld.X > panelCavas.Width / 2 + 50
                    && RightBottomPanel.Location.Y - startPoint.Y + pOld.Y > panelCavas.Height / 2 + 50)
                {
                    RightBottomPanel.Location = new Point(RightBottomPanel.Location.X - startPoint.X + pOld.X, RightBottomPanel.Location.Y - startPoint.Y + pOld.Y);
                    SketchBox.Size = new Size(SketchBox.Width - startPoint.X + pOld.X, SketchBox.Height - startPoint.Y + pOld.Y);
                }    
                
                bm = new Bitmap(SketchBox.Width, SketchBox.Height);
                gra = Graphics.FromImage(bm);
                gra.Clear(Color.White);
                gra.DrawImage(temp, 0, 0, temp.Width, temp.Height);
                SketchBox.BackgroundImage = (Bitmap)bm.Clone();
            }
        }

        private void RightBottomPanel_1_MouseUp(object sender, MouseEventArgs e)
        {
            isDragged = false;
        }

    }
}
