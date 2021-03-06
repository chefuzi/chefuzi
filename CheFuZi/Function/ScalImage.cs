﻿using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheFuZi.Function
{
    public class ScalImage
    {
        private bool _isOldStyle = true;
        private string imageOldPath = "";
        private string imageNewPath = "";
        private int width = 150;
        private int height = 150;
        /// <summary>
        /// TRUE原图按大小比例，FALSE按给定大小缩放
        /// </summary>
        public bool isOldStyle
        {
            set { _isOldStyle = value; }
            get { return _isOldStyle; }
        }
        /// <summary>
        /// 原图片地址
        /// </summary>
        public string ImageOldPath
        {
            set { imageOldPath = value; }
            get { return imageOldPath; }
        }
        /// <summary>
        /// 新图片地址
        /// </summary>
        public string ImageNewPath
        {
            set { imageNewPath = value; }
            get { return imageNewPath; }
        }
        /// <summary>
        /// 缩略图宽度
        /// </summary>
        public int ImageNewWidth
        {
            set { width = value; }
            get { return width; }
        }
        /// <summary>
        /// 缩略图高度
        /// </summary>
        public int ImageNewHeight
        {
            set { height = value; }
            get { return height; }
        }
        //// <summary>  
        /// 生成缩略图  
        /// </summary>  
        public void scalImage()
        {
            if (File.Exists(imageOldPath))
            {
                //获取原始图片  
                Image originalImage = Image.FromFile(imageOldPath);
                //缩略图画布宽高  
                int towidth = width;
                int toheight = height;
                //原始图片写入画布坐标和宽高(用来设置裁减溢出部分)  
                int x = 0;
                int y = 0;
                int ow = originalImage.Width;
                int oh = originalImage.Height;
                //原始图片画布,设置写入缩略图画布坐标和宽高(用来原始图片整体宽高缩放)  
                int bg_x = 0;
                int bg_y = 0;
                int bg_w = towidth;
                int bg_h = toheight;
                //倍数变量  
                double multiple = 0;
                //获取宽长的或是高长与缩略图的倍数  
                if (originalImage.Width >= originalImage.Height) multiple = (double)originalImage.Width / (double)width;
                else multiple = (double)originalImage.Height / (double)height;
                //上传的图片的宽和高小等于缩略图  
                if (ow <= width && oh <= height)
                {
                    //缩略图按原始宽高  
                    bg_w = originalImage.Width;
                    bg_h = originalImage.Height;
                    //空白部分用背景色填充  
                    bg_x = Convert.ToInt32(((double)towidth - (double)ow) / 2);
                    bg_y = Convert.ToInt32(((double)toheight - (double)oh) / 2);
                }
                //上传的图片的宽和高大于缩略图  
                else
                {
                    //宽高按比例缩放  
                    bg_w = Convert.ToInt32((double)originalImage.Width / multiple);
                    bg_h = Convert.ToInt32((double)originalImage.Height / multiple);
                    //空白部分用背景色填充  
                    bg_y = Convert.ToInt32(((double)height - (double)bg_h) / 2);
                    bg_x = Convert.ToInt32(((double)width - (double)bg_w) / 2);
                }

                //新建一个bmp图片,并设置缩略图大小.  
                System.Drawing.Image bitmap;
                if (_isOldStyle)
                {//安原图比例
                    bitmap = new System.Drawing.Bitmap(bg_w, bg_h);
                }
                else
                {//按给定大小比例
                    bitmap = new System.Drawing.Bitmap(towidth, toheight);
                }
                //新建一个画板  
                System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap);
                //设置高质量插值法  
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;
                //设置高质量,低速度呈现平滑程度  
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                //清空画布并设置背景色  
                g.Clear(System.Drawing.ColorTranslator.FromHtml("#ffffff"));
                //在指定位置并且按指定大小绘制原图片的指定部分  
                //第一个System.Drawing.Rectangle是原图片的画布坐标和宽高,第二个是原图片写在画布上的坐标和宽高,最后一个参数是指定数值单位为像素  
                if (_isOldStyle)
                {//安原图比例
                    g.DrawImage(originalImage, new System.Drawing.Rectangle(x, y, bg_w, bg_h), new System.Drawing.Rectangle(x, y, ow, oh), System.Drawing.GraphicsUnit.Pixel);
                }
                else
                {//按给定大小比例
                    g.DrawImage(originalImage, new System.Drawing.Rectangle(bg_x, bg_y, bg_w, bg_h), new System.Drawing.Rectangle(x, y, ow, oh), System.Drawing.GraphicsUnit.Pixel);
                }
                try
                {
                    if (File.Exists(imageNewPath))
                    {
                        File.Delete(imageNewPath);
                    }
                    //获取图片类型  
                    string fileExtension = System.IO.Path.GetExtension(imageOldPath).ToLower();
                    //按原图片类型保存缩略图片,不按原格式图片会出现模糊,锯齿等问题.  
                    switch (fileExtension)
                    {
                        case ".gif": bitmap.Save(imageNewPath, System.Drawing.Imaging.ImageFormat.Gif); break;
                        case ".jpg": bitmap.Save(imageNewPath, System.Drawing.Imaging.ImageFormat.Jpeg); break;
                        case ".jpeg": bitmap.Save(imageNewPath, System.Drawing.Imaging.ImageFormat.Jpeg); break;
                        case ".bmp": bitmap.Save(imageNewPath, System.Drawing.Imaging.ImageFormat.Bmp); break;
                        case ".png": bitmap.Save(imageNewPath, System.Drawing.Imaging.ImageFormat.Png); break;
                    }
                }
                catch (System.Exception e)
                {
                    throw e;
                }
                finally
                {
                    originalImage.Dispose();
                    bitmap.Dispose();
                    g.Dispose();
                }
            }
        }
    }
}
