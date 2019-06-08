using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Windows.Forms;//该命名空间需要导入System.Windows.Forms.dll
                           //可在路径：Unity安装目录/Editor\Data\Mono\lib\mono\2.0里找到
using System.Drawing;//该命名空间需要导入System.Drawing.dll
                     //可在路径：Unity安装目录/Editor\Data\Mono\lib\mono\2.0里找到
using System;

public class Tray
{
    //NotifyIcon 设置托盘相关参数
    private static NotifyIcon _notifyIcon = new NotifyIcon();
    //托盘图标的宽高
    private static int _width = 40, _height = 40;
    //做托盘图标的图片，这里用了.png格式
    private static Texture2D iconTexture2D;

    //调用该方法将运行程序显示到托盘
    public static void InitTray()
    {
        _notifyIcon.BalloonTipText = "";//托盘气泡显示内容
        _notifyIcon.Text = "这是托盘提示信息";
        _notifyIcon.Visible = false;//托盘按钮是否可见
        //_notifyIcon.Icon = new System.Drawing.Icon(SystemIcons.Information, 40, 40);
        /*
        特别注明：这里是一个小坑，一开始我把图片放在Resources目录下，然后用Resources.Load加载
        然后运行会报错：Texture2D::EncodeTo functions do not support compressed texture formats.（大意是编码成字节的方法不支持压缩图片），怎么设置都不好使
        这一刻，我想起了以前了解过的知识。灵机一动，建了一个StreamingAssets目录（Unity不会对该目录进行任何操作），把图片放里面。Done！
        */
        _notifyIcon.Icon = CustomTrayIcon(@UnityEngine.Application.streamingAssetsPath + "/icon.png", _width, _height);
        _notifyIcon.ShowBalloonTip(2000);//托盘气泡显示时间
        _notifyIcon.MouseDoubleClick += notifyIcon_MouseDoubleClick;//双击托盘图标响应事件
    }
    /// <summary>
    /// 设置程序托盘图标
    /// </summary>
    /// <param name="iconPath">图标路径</param>
    /// <param name="width">宽</param>
    /// <param name="height">高</param>
    /// <returns>图标</returns>
    private static Icon CustomTrayIcon(string iconPath, int width, int height)
    {
        Bitmap bt = new Bitmap(iconPath);
        Bitmap fitSizeBt = new Bitmap(bt, width, height);
        return Icon.FromHandle(fitSizeBt.GetHicon());
    }
    private static Icon CustomTrayIcon(System.Drawing.Image img, int width, int height)
    {
        Bitmap bt = new Bitmap(img);
        Bitmap fitSizeBt = new Bitmap(bt, width, height);
        return Icon.FromHandle(fitSizeBt.GetHicon());
    }
    /// <summary>
    /// byte[]转换成Image
    /// </summary>
    /// <param name="byteArrayIn">二进制图片流</param>
    /// <returns>Image</returns>
    public static System.Drawing.Image ByteArrayToImage(byte[] byteArrayIn)
    {
        if (byteArrayIn == null)
            return null;
        using (System.IO.MemoryStream ms = new System.IO.MemoryStream(byteArrayIn))
        {
            System.Drawing.Image returnImage = System.Drawing.Image.FromStream(ms);
            ms.Flush();
            return returnImage;
        }
    }



    public static void DestroyDoubleClick()
    {
        _notifyIcon.MouseDoubleClick -= notifyIcon_MouseDoubleClick;
    }

    /// <summary>
    /// 显示托盘图标
    /// </summary>
    public static void ShowTray()
    {
        _notifyIcon.Visible = true;//托盘按钮是否可见
    }

    /// <summary>
    /// 双击托盘图标、程序最大化、并 托盘图标隐藏
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private static void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
           // WindowsForm.OnClickMaximize();
            _notifyIcon.Visible = false;//托盘按钮是否可见
        }
    }
}