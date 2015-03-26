using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Media.Imaging;
using DRR_pasori;
using CSJ2K;
using Windows.UI.Popups;
using System.Text.RegularExpressions;

// 空白ページのアイテム テンプレートについては、http://go.microsoft.com/fwlink/?LinkId=234238 を参照してください

namespace DDR_store
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class MainPage : Page
    {

        Scard sc;


        public MainPage()
        {
            this.InitializeComponent();
            ButtonStop.Visibility = Visibility.Collapsed;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (textBoxPass1.Password.Length == 0)
            {
                var dialog = new MessageDialog("暗証番号１が未入力です");
                await dialog.ShowAsync();
                return;
            }
            if (textBoxPass1.Password.Length < 4)
            {
                var dialog = new MessageDialog("暗証番号１は４桁必要です");
                await dialog.ShowAsync();
                return;
            }
            if (!((Regex.Match(textBoxPass1.Password, "^[0-9¥*]+$")).Success))
            {
                var dialog = new MessageDialog("暗証番号１に、使用できない文字が含まれています");
                await dialog.ShowAsync();
                return;
            }
            if (textBoxPass2.Password.Length == 0)
            {
                var dialog = new MessageDialog("暗証番号２が未入力です");
                await dialog.ShowAsync();
                return;
            }
            if (textBoxPass2.Password.Length < 4)
            {
                var dialog = new MessageDialog("暗証番号２は４桁必要です");
                await dialog.ShowAsync();
                return;
            }
            if (!((Regex.Match(textBoxPass2.Password, "^[0-9¥*]+$")).Success))
            {
                var dialog = new MessageDialog("暗証番号２に、使用できない文字が含まれています");
                await dialog.ShowAsync();
                return;
            }

            sc = new Scard();
            sc.CardEvent += CardEventHandler;
            sc.RWnameEvent += RWnameEventHandler;
            bool b = await sc.start(textBoxPass1.Password, textBoxPass2.Password);


            if (b == false)
            {
                var dialog = new MessageDialog(sc.errormsg);
                await dialog.ShowAsync();
                return;
            }
            textBoxPass1.IsEnabled = false;
            textBoxPass2.IsEnabled = false;
            ButtonRead.Visibility = Visibility.Collapsed;
            ButtonStop.Visibility = Visibility.Visible;

        }

        private void ButtonStop_Click(object sender, RoutedEventArgs e)
        {
            sc.stop();
        }




        private String ConvertNego(String topone)
        {
            String ret = "昭和";
            String one = topone.Substring(0, 1);
            String year = topone.Substring(1, 2);
            String month = topone.Substring(3, 2);
            String day = topone.Substring(5, 2);

            if (one.CompareTo("1") == 0)
            {
                ret = "明治";
            }
            if (one.CompareTo("2") == 0)
            {
                ret = "大正";
            }
            if (one.CompareTo("3") == 0)
            {
                ret = "昭和";
            }
            if (one.CompareTo("4") == 0)
            {
                ret = "平成";
            }
            ret = ret + year + "年" + month + "月" + day + "日";

            return ret;
        }

        void RWnameEventHandler(String name)
        {
            this.CardRWtext.Text = name;

        }


        void CardEventHandler(object sender)
        {
            Scard sc = (Scard)sender;

            this.ButtonRead.Visibility = Visibility.Visible;
            this.ButtonStop.Visibility = Visibility.Collapsed;
            this.textBoxPass1.IsEnabled = true;
            this.textBoxPass2.IsEnabled = true;

            if (!sc.RWresult)
            {
                return;
            }
            labelName.Text = sc.name;
            labelYomikana.Text = sc.kana;
            labelTusyo.Text = sc.tusyo;
            labelToitu.Text = sc.toitsu;
            labelBirth.Text = ConvertNego(sc.birth);
            labelAdr.Text = sc.address;
            labelHonseki.Text = sc.honseki;
            labelKubun.Text = sc.kubun;
            labelNumber.Text = sc.menkyonumber;
            labelJyoken1.Text = sc.joken1;
            labelJyoken2.Text = sc.joken2;
            labelJyoken3.Text = sc.joken3;
            labelJyoken4.Text = sc.joken4;
            labelKigen.Text = ConvertNego(sc.yukoday);
            labelKoan.Text = sc.koanname;

            labelNisyogen.Text = ConvertNego(sc.nisyogenday);    // 免許の年月日(二・小・原)(元号(注6)YYMMDD)
            labelHoka.Text = ConvertNego(sc.hokaday);            // 免許の年月日(他)(元号(注6)YYMMDD)(注9
            labelNisyu.Text = ConvertNego(sc.nisyuday);          // 免許の年月日(二種)(元号(注6)YYMMDD)(注9)
            labelOgata.Text = ConvertNego(sc.ogataday);          // 免許の年月日(大型)(元号(注6)YYMMDD)(注9)
            labelFutu.Text = ConvertNego(sc.futuday);            // 免許の年月日(普通)(元号(注6)YYMMDD)(注9)
            labelDaitoku.Text = ConvertNego(sc.daitokuday);      // 免許の年月日(大特)(元号(注6)YYMMDD)(注9)
            labelDaijini.Text = ConvertNego(sc.daijiniday);      // 免許の年月日(大自二)(元号(注6)YYMMDD)(注9)
            labelFujini.Text = ConvertNego(sc.futujiniday);      // 免許の年月日(普自二)(元号(注6)YYMMDD)(注9)
            labelSyotoku.Text = ConvertNego(sc.kotokuday);       // 免許の年月日(小特)(元号(注6)YYMMDD)(注9)
            labelGentuki.Text = ConvertNego(sc.gentukiday);      // 免許の年月日(原付)(元号(注6)YYMMDD)(注9)
            labelKenbiki.Text = ConvertNego(sc.keninday);        // 免許の年月日(け引)(元号(注6)YYMMDD)(注9)
            labelDaini.Text = ConvertNego(sc.daijiday);          // 免許の年月日(大二)(元号(注6)YYMMDD)(注9)   
            labelDaitokuni.Text = ConvertNego(sc.daitokuji);     // 免許の年月日(大特二)(元号(注6)YYMMDD)(注9)           
            labelkehikini.Text = ConvertNego(sc.keninniday);     // 免許の年月日(け引二)(元号(注6)YYMMDD)(注9)
            labelChugata.Text = ConvertNego(sc.chuday);          // 免許の年月日(中型)(元号(注6)YYMMDD)(注9,注12)
            labelChuni.Text = ConvertNego(sc.chuniday);          // 免許の年月日(中二)(元号(注6)YYMMDD)(注9,注12)
            labelFuni.Text = ConvertNego(sc.fujiday);          // 免許の年月日(普二)(元号(注6)YYMMDD)(注9)    

            CSJ2KSetup.RegisterCreators();
            WriteableBitmap face = (WriteableBitmap)J2kImage.FromBytes(sc.picture);
            Picture1.Source = face;

        }

    }
}
