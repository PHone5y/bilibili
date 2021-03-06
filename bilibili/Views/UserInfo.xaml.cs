﻿using System;
using System.Collections.Generic;
using Windows.Data.Json;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;
using Windows.Web.Http.Filters;
using bilibili.Helpers;
using bilibili.Http;
using bilibili.Methods;
using bilibili.Models;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace bilibili.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class UserInfo : Page
    {
        List<Folder> myFolder = new List<Folder>();
        public UserInfo()
        {
            this.InitializeComponent();
            load();          
        }

        async void load()
        {
            try
            {
                if (SettingHelper.GetValue("_accesskey").ToString().Length > 2)
                {
                    string url = "http://api.bilibili.com/myinfo?appkey=" + ApiHelper.appkey + "&access_key=" + ApiHelper.accesskey;
                    url += ApiHelper.GetSign(url);
                    JsonObject json = await BaseService.GetJson(url);
                    if (json.ContainsKey("face"))
                        Face.Source = new BitmapImage { UriSource = new Uri(StringDeal.delQuotationmarks((json["face"].ToString()))) };
                    if (json.ContainsKey("coins"))
                        coins.Text += json["coins"].ToString();
                    if (json.ContainsKey("sign"))
                        sign.Text = StringDeal.delQuotationmarks(json["sign"].ToString());
                    if (json.ContainsKey("uname"))
                        userName.Text = StringDeal.delQuotationmarks(json["uname"].ToString());
                    if (json.ContainsKey("level_info"))
                    {
                        JsonObject json2 = JsonObject.Parse(json["level_info"].ToString());
                        if (json2.ContainsKey("next_exp"))
                        {
                            exp_total.Text = json2["next_exp"].ToString();
                            bar.Maximum = Convert.ToInt32(json2["next_exp"].ToString());
                        }
                        if (json2.ContainsKey("current_exp"))
                        {
                            exp_current.Text = json2["current_exp"].ToString();
                            bar.Value = Convert.ToInt32(json2["current_exp"].ToString());
                        }
                        if (json2.ContainsKey("current_level"))
                        {
                            level.Source = new BitmapImage { UriSource = new Uri("ms-appx:///Assets//Others//lv" + json2["current_level"].ToString() + ".png", UriKind.Absolute) };
                        }
                    }
                }
                myFolder = await ContentServ.GetFavFolders();
                foreach (var item in await ContentServ.GetConAsync(1))
                {
                    conlist.Items.Add(item);
                }
                int count = 0;
                foreach (var item in myFolder)
                {
                    count += int.Parse(item.Count);
                }
                if (count == 0)
                    fav.Content += "（暂无收藏）";
            }
            catch
            {
                
            }
        }

        private async void logout_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (ApiHelper.IsLogin())
            {
                ContentDialog dialog = new ContentDialog
                {
                    Content = "确定登出吗？",
                    IsPrimaryButtonEnabled = true,
                    IsSecondaryButtonEnabled = true,
                    PrimaryButtonText = "确定",
                    SecondaryButtonText = "手滑了",
                };
                dialog.PrimaryButtonClick += Dialog_PrimaryButtonClick;
                await dialog.ShowAsync();
            }
        }
        private void Dialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            ApiHelper.logout();
        }

        private void Button_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MyConcerns),null,new Windows.UI.Xaml.Media.Animation.SlideNavigationTransitionInfo());
        }

        private void conlist_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            GridView ctrl = sender as GridView;
            var item = ctrl.SelectedItem;
            if (item != null)
            {
                Frame.Navigate(typeof(Detail), (item as Concern).ID, new Windows.UI.Xaml.Media.Animation.DrillInNavigationTransitionInfo());
            }
        }

        private async void coin_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (ApiHelper.IsLogin())
            {
                Dialogs.CoinHistory ch = new Dialogs.CoinHistory();
                await ch.ShowAsync();
            }
        }

        private void fav_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Frame.Navigate(typeof(FavCollection), null, new Windows.UI.Xaml.Media.Animation.SlideNavigationTransitionInfo());
        }
    }
}
