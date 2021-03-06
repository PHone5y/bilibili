﻿using bilibili.Helpers;
using bilibili.Http;
using bilibili.Methods;
using Windows.UI.Xaml;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.Data.Json;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace bilibili.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class History : Page
    {
        int page = 1;
        bool isDone = false;
        public History()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            await load(1);
        }
        //http://api.bilibili.com/x/v2/history?_device=android&_hwid=ccbb856c97ccb8d2&_ulv=10000&access_key=a48208a7c5b67a4a369124cf5c1b515c&appkey=1d8b6e7d45233436&build=427000&mobi_app=android&platform=android&pn=1&ps=200

        async Task load(int p)
        {
            try
            {
                string url = "http://api.bilibili.com/x/v2/history?_device=wp&_ulv=10000&access_key=" + ApiHelper.accesskey + "&appkey=" + ApiHelper.appkey + "&build=427000&platform=android&pn=" + p.ToString() + "&ps=20";
                url += ApiHelper.GetSign(url);
                List<Models.History> hs = await ContentServ.GetHistoryAsync(url);
                foreach (var item in hs)
                {
                    hslist.Items.Add(item);
                }
                if (hs.Count < 20)
                {
                    var text = Load.FindChildOfType<TextBlock>(hslist);
                    text.Text = "没有更多历史项";
                    isDone = true;
                }
            }
            catch { }
        }

        private void hslist_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            var scroll = Load.FindChildOfType<ScrollViewer>(hslist);
            var text = Load.FindChildOfType<TextBlock>(hslist);
            scroll.ViewChanged += async (s, a) =>
            {
                if (scroll.VerticalOffset == scroll.ScrollableHeight && isDone == false) 
                {
                    int count0 = hslist.Items.Count;
                    //滑动到底部了    
                    text.Visibility = Visibility.Visible;
                    page++;
                    await load(page);
                    if (hslist.Items.Count == count0)
                    {
                        text.Text = "没有更多历史项！";
                        return;
                    }
                    text.Visibility = Visibility.Collapsed;
                }
            };
        }

        private void hslist_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            Models.History item = hslist.SelectedItem as Models.History;
            if (item != null)
            {
                Frame.Navigate(typeof(Detail_P), item.Aid, new Windows.UI.Xaml.Media.Animation.DrillInNavigationTransitionInfo());
            }
        }

        private async void clear_Click(object sender, RoutedEventArgs e)
        {
            //http://api.bilibili.com/x/v2/history/clear?_device=android&access_key=c0ca6415ce6d8bcb7bda0ea9bc9a2419&appkey=c1b107428d337928&build=421000&mobi_app=android&platform=android&sign=fe59b1a3abe6d935094e757a8a718424
            string url = "http://api.bilibili.com/x/v2/history/clear?_device=android&access_key=" + ApiHelper.accesskey + "&appkey=c1b107428d337928&build=421000&mobi_app=android&platform=android";
            url += ApiHelper.GetSign(url);
            //B站真有趣，清除历史记录好好的GET不用非用POST……
            JsonObject json = JsonObject.Parse(await BaseService.SendPostAsync(url, ""));
            if (json.ContainsKey("code"))
            {
                if (json["code"].ToString() == "0")
                {
                    hslist.Items.Clear();
                }
            }
        }
    }
}
