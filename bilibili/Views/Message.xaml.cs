﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using bilibili.Http;
using bilibili.Models;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace bilibili.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class Message : Page
    {
        bool IsChatLoaded = false;
        bool IsNotiLoaded = false;
        bool IsWissLoaded = false;
        public Message()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            await loadStatus();
        }

        private async Task loadStatus()
        {
            //http://message.bilibili.com/api/notify/query.replyme.list.do?captcha=e91dd48f73e11db1cc7c3b729865e572&data_type=1
            Count count = await ContentServ.GetCountAsync();
            if (count != null)
            {
                h0.Text += count.Reply_me == "0" ? string.Empty : count.Reply_me;
                h1.Text += count.Chat_me == "0" ? string.Empty : count.Chat_me;
                h2.Text += count.Notify_me == "0" ? string.Empty : count.Notify_me;
                int a = int.Parse(count.At_me) + int.Parse(count.Praise_me);
                h3.Text += a == 0 ? string.Empty : a.ToString();
            }
        }

        private async void mainpivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TextBlock txt = this.FindName(string.Format("h{0}", mainpivot.SelectedIndex)) as TextBlock;
            for (int i = 0; i < mainpivot.Items.Count; i++)
            {
                TextBlock temp = this.FindName(string.Format("h{0}", i)) as TextBlock;
                temp.Foreground = new SolidColorBrush(Colors.LightGray);
            }
            txt.Foreground = new SolidColorBrush(Colors.White);
            switch(mainpivot.SelectedIndex)
            {
                case 0:
                    {
                        if (!IsChatLoaded)
                        {
                            List<Chat> chats = await ContentServ.GetChatsAsync();
                            ls_chat.ItemsSource = chats;
                            IsChatLoaded = true;
                        }
                    }break;
                case 1:
                    {
                        if (!IsWissLoaded)
                        {
                            List<Wisper> wiss = await ContentServ.GetWisperAsync();
                            ls_wiss.ItemsSource = wiss;
                            IsWissLoaded = true;
                        }
                    }
                    break;
                case 2:
                    {
                        if (!IsNotiLoaded)
                        {
                            List<Notify> notis = await ContentServ.GetNotiAsync();
                            ls_noti.ItemsSource = notis;
                            IsNotiLoaded = true;
                        }
                    }
                    break;
            }
        }

        private void ls_chat_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem as Chat != null)
            {
                Frame.Navigate(typeof(Detail_P), (e.ClickedItem as Chat).Aid, new Windows.UI.Xaml.Media.Animation.DrillInNavigationTransitionInfo());
            }
        }
    }
}
