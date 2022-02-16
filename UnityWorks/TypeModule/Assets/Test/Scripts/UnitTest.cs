using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using tm;
using tm.inner;
using UnityEditor;

///<summary>TypeModuleのテスト用</summary>
public class UnitTest : MonoBehaviour{

    #region Unity共通処理
    void Start(){
        if (IsTest) {
            //Util=================
            TestUtil();
            TestCsvReadHelper();

            //Converts=================
            TestConvertTableMgr();
            TestKana2KanaMidTable();
            TestKana2RomaTable();
            TestKanaMid2KanaTable();
            TestKeyCode2CharTable();
            TestRoma2KanaTable();
        }
    }

    void Update(){
        
    }
    #endregion


    #region 単体テスト
    /// <summary>Util テスト</summary>
    void TestUtil() {
        
        Debug.Log("Test Util");
        Debug.Assert(string.Compare(Util.KatakanaToHiragana("あいうえおかきくけこばぴゔんー"), "あいうえおかきくけこばぴゔんー") == 0);
        Debug.Assert(string.Compare(Util.KatakanaToHiragana("アイウエオカキクケコバピヴンー"), "あいうえおかきくけこばぴゔんー") == 0);
        Debug.Assert(string.Compare(Util.KatakanaToHiragana("ｱｲｳｴｵｶｷｸｹｺﾊﾞﾋﾟｳﾞﾝｰ"), "あいうえおかきくけこばぴゔんー") == 0);
        Debug.Assert(string.Compare(Util.KatakanaToHiragana("あイウｴｵカキクケコばﾋﾟゔンー"), "あいうえおかきくけこばぴゔんー") == 0);
        Debug.Assert(string.Compare(Util.KatakanaToHiragana("09ABCxyz漢字０９ａｂｃＸＹＺあゔんアヴンｱｳﾞﾝーｰ"), "09ABCxyz漢字０９ａｂｃＸＹＺあゔんあゔんあゔんーー") == 0);

        Debug.Assert(string.Compare(Util.HanToZen(
            "09ABCxyz漢字０９ａｂｃＸＹＺあゔんアヴンｱｳﾞﾝ!\"#$%&'()=~|-^\\@[`{;:]+*},./<>_", Util.ConvertTypes.All),
            "０９ＡＢＣｘｙｚ漢字０９ａｂｃＸＹＺあゔんアヴンアヴン！”＃＄％＆’（）＝～｜ー＾￥＠「‘｛；：」＋＊｝、。／＜＞＿") == 0);
        Debug.Assert(string.Compare(Util.HanToZen(
            "09ABCxyz漢字０９ａｂｃＸＹＺあゔんアヴンｱｳﾞﾝ!\"#$%&'()=~|-^\\@[`{;:]+*},./<>_", Util.ConvertTypes.Alphabet),
            "09ＡＢＣｘｙｚ漢字０９ａｂｃＸＹＺあゔんアヴンｱｳﾞﾝ!\"#$%&'()=~|-^\\@[`{;:]+*},./<>_") == 0);
        Debug.Assert(string.Compare(Util.HanToZen(
            "09ABCxyz漢字０９ａｂｃＸＹＺあゔんアヴンｱｳﾞﾝ!\"#$%&'()=~|-^\\@[`{;:]+*},./<>_", Util.ConvertTypes.Number),
            "０９ABCxyz漢字０９ａｂｃＸＹＺあゔんアヴンｱｳﾞﾝ!\"#$%&'()=~|-^\\@[`{;:]+*},./<>_") == 0);
        Debug.Assert(string.Compare(Util.HanToZen(
            "09ABCxyz漢字０９ａｂｃＸＹＺあゔんアヴンｱｳﾞﾝ!\"#$%&'()=~|-^\\@[`{;:]+*},./<>_", Util.ConvertTypes.Symbol),
            "09ABCxyz漢字０９ａｂｃＸＹＺあゔんアヴンｱｳﾞﾝ！”＃＄％＆’（）＝～｜ー＾￥＠「‘｛；：」＋＊｝、。／＜＞＿") == 0);
        Debug.Assert(string.Compare(Util.HanToZen(
            "09ABCxyz漢字０９ａｂｃＸＹＺあゔんアヴンｱｳﾞﾝ!\"#$%&'()=~|-^\\@[`{;:]+*},./<>_", Util.ConvertTypes.Katakana),
            "09ABCxyz漢字０９ａｂｃＸＹＺあゔんアヴンアヴン!\"#$%&'()=~|ー^\\@[`{;:]+*},./<>_") == 0);
        Debug.Assert(string.Compare(Util.HanToZen(
            "09ABCxyz漢字０９ａｂｃＸＹＺあゔんアヴンｱｳﾞﾝ!\"#$%&'()=~|-^\\@[`{;:]+*},./<>_", Util.ConvertTypes.Katakana | Util.ConvertTypes.Symbol),
            "09ABCxyz漢字０９ａｂｃＸＹＺあゔんアヴンアヴン！”＃＄％＆’（）＝～｜ー＾￥＠「‘｛；：」＋＊｝、。／＜＞＿") == 0);

        Debug.Assert(string.Compare(Util.ZenToHan(
            "09ABCxyz漢字０９ａｂｃＸＹＺあゔんアヴンｱｳﾞﾝ！”＃＄％＆’（）＝～｜ー＾￥＠「‘｛；：」＋＊｝、。／＜＞＿", Util.ConvertTypes.All),
            "09ABCxyz漢字09abcXYZあゔんｱｳﾞﾝｱｳﾞﾝ!\"#$%&\'()=~|-^\\@[`{;:]+*},./<>_") == 0);
        Debug.Assert(string.Compare(Util.ZenToHan(
            "09ABCxyz漢字０９ａｂｃＸＹＺあゔんアヴンｱｳﾞﾝ！”＃＄％＆’（）＝～｜ー＾￥＠「‘｛；：」＋＊｝、。／＜＞＿", Util.ConvertTypes.Alphabet),
            "09ABCxyz漢字０９abcXYZあゔんアヴンｱｳﾞﾝ！”＃＄％＆’（）＝～｜ー＾￥＠「‘｛；：」＋＊｝、。／＜＞＿") == 0);
        Debug.Assert(string.Compare(Util.ZenToHan(
            "09ABCxyz漢字０９ａｂｃＸＹＺあゔんアヴンｱｳﾞﾝ！”＃＄％＆’（）＝～｜ー＾￥＠「‘｛；：」＋＊｝、。／＜＞＿", Util.ConvertTypes.Number),
            "09ABCxyz漢字09ａｂｃＸＹＺあゔんアヴンｱｳﾞﾝ！”＃＄％＆’（）＝～｜ー＾￥＠「‘｛；：」＋＊｝、。／＜＞＿") == 0);
        Debug.Assert(string.Compare(Util.ZenToHan(
            "09ABCxyz漢字０９ａｂｃＸＹＺあゔんアヴンｱｳﾞﾝ！”＃＄％＆’（）＝～｜ー＾￥＠「‘｛；：」＋＊｝、。／＜＞＿", Util.ConvertTypes.Symbol),
            "09ABCxyz漢字０９ａｂｃＸＹＺあゔんアヴンｱｳﾞﾝ!\"#$%&\'()=~|-^\\@[`{;:]+*},./<>_") == 0);
        Debug.Assert(string.Compare(Util.ZenToHan(
            "09ABCxyz漢字０９ａｂｃＸＹＺあゔんアヴンｱｳﾞﾝ！”＃＄％＆’（）＝～｜ー＾￥＠「‘｛；：」＋＊｝、。／＜＞＿", Util.ConvertTypes.Katakana),
            "09ABCxyz漢字０９ａｂｃＸＹＺあゔんｱｳﾞﾝｱｳﾞﾝ！”＃＄％＆’（）＝～｜-＾￥＠「‘｛；：」＋＊｝、。／＜＞＿") == 0);
        Debug.Assert(string.Compare(Util.ZenToHan(
            "09ABCxyz漢字０９ａｂｃＸＹＺあゔんアヴンｱｳﾞﾝ！”＃＄％＆’（）＝～｜ー＾￥＠「‘｛；：」＋＊｝、。／＜＞＿", Util.ConvertTypes.Number | Util.ConvertTypes.Alphabet),
            "09ABCxyz漢字09abcXYZあゔんアヴンｱｳﾞﾝ！”＃＄％＆’（）＝～｜ー＾￥＠「‘｛；：」＋＊｝、。／＜＞＿") == 0);

        Debug.Assert(string.Compare(Util.HanToZenNum(
            "09ABCxyz漢字０９ａｂｃＸＹＺあゔんアヴンｱｳﾞﾝ！”＃＄％＆’（）＝～｜ー＾￥＠「‘｛；：」＋＊｝、。／＜＞＿"),
            "０９ABCxyz漢字０９ａｂｃＸＹＺあゔんアヴンｱｳﾞﾝ！”＃＄％＆’（）＝～｜ー＾￥＠「‘｛；：」＋＊｝、。／＜＞＿") == 0);

        Debug.Assert(string.Compare(Util.ZenToHanNum(
            "09ABCxyz漢字０９ａｂｃＸＹＺあゔんアヴンｱｳﾞﾝ！”＃＄％＆’（）＝～｜ー＾￥＠「‘｛；：」＋＊｝、。／＜＞＿"),
            "09ABCxyz漢字09ａｂｃＸＹＺあゔんアヴンｱｳﾞﾝ！”＃＄％＆’（）＝～｜ー＾￥＠「‘｛；：」＋＊｝、。／＜＞＿") == 0);

        Debug.Assert(string.Compare(Util.HanToZenAlpha(
            "09ABCxyz漢字０９ａｂｃＸＹＺあゔんアヴンｱｳﾞﾝ！”＃＄％＆’（）＝～｜ー＾￥＠「‘｛；：」＋＊｝、。／＜＞＿"),
            "09ＡＢＣｘｙｚ漢字０９ａｂｃＸＹＺあゔんアヴンｱｳﾞﾝ！”＃＄％＆’（）＝～｜ー＾￥＠「‘｛；：」＋＊｝、。／＜＞＿") == 0);

        Debug.Assert(string.Compare(Util.ZenToHanAlpha(
            "09ABCxyz漢字０９ａｂｃＸＹＺあゔんアヴンｱｳﾞﾝ！”＃＄％＆’（）＝～｜ー＾￥＠「‘｛；：」＋＊｝、。／＜＞＿"),
            "09ABCxyz漢字０９abcXYZあゔんアヴンｱｳﾞﾝ！”＃＄％＆’（）＝～｜ー＾￥＠「‘｛；：」＋＊｝、。／＜＞＿") == 0);

        Debug.Assert(string.Compare(Util.HanToZenAlNum(
            "09ABCxyz漢字０９ａｂｃＸＹＺあゔんアヴンｱｳﾞﾝ！”＃＄％＆’（）＝～｜ー＾￥＠「‘｛；：」＋＊｝、。／＜＞＿"),
            "０９ＡＢＣｘｙｚ漢字０９ａｂｃＸＹＺあゔんアヴンｱｳﾞﾝ！”＃＄％＆’（）＝～｜ー＾￥＠「‘｛；：」＋＊｝、。／＜＞＿") == 0);

        Debug.Assert(string.Compare(Util.ZenToHanAlNum(
            "09ABCxyz漢字０９ａｂｃＸＹＺあゔんアヴンｱｳﾞﾝ！”＃＄％＆’（）＝～｜ー＾￥＠「‘｛；：」＋＊｝、。／＜＞＿"),
            "09ABCxyz漢字09abcXYZあゔんアヴンｱｳﾞﾝ！”＃＄％＆’（）＝～｜ー＾￥＠「‘｛；：」＋＊｝、。／＜＞＿") == 0);


        Debug.Assert(Util.IsHiragana('0') == false);
        Debug.Assert(Util.IsHiragana('9') == false);
        Debug.Assert(Util.IsHiragana('A') == false);
        Debug.Assert(Util.IsHiragana('z') == false);
        Debug.Assert(Util.IsHiragana('漢') == false);
        Debug.Assert(Util.IsHiragana('０') == false);
        Debug.Assert(Util.IsHiragana('９') == false);
        Debug.Assert(Util.IsHiragana('ａ') == false);
        Debug.Assert(Util.IsHiragana('Ｚ') == false);
        Debug.Assert(Util.IsHiragana('あ') == true);
        Debug.Assert(Util.IsHiragana('ゔ') == true);
        Debug.Assert(Util.IsHiragana('ん') == true);
        Debug.Assert(Util.IsHiragana('ば') == true);
        Debug.Assert(Util.IsHiragana('ぱ') == true);
        Debug.Assert(Util.IsHiragana('ー') == true);
        Debug.Assert(Util.IsHiragana('ア') == false);
        Debug.Assert(Util.IsHiragana('ン') == false);
        Debug.Assert(Util.IsHiragana('ヴ') == false);
        Debug.Assert(Util.IsHiragana('ｱ') == false);
        Debug.Assert(Util.IsHiragana('ｳ') == false);
        Debug.Assert(Util.IsHiragana('ﾝ') == false);
        Debug.Assert(Util.IsHiragana('ー') == true);
        Debug.Assert(Util.IsHiragana('-') == false);
        Debug.Assert(Util.IsHiragana('!') == false);
        Debug.Assert(Util.IsHiragana('*') == false);
        Debug.Assert(Util.IsHiragana('－') == false);
        Debug.Assert(Util.IsHiragana('！') == false);
        Debug.Assert(Util.IsHiragana('＊') == false);
        Debug.Assert(Util.IsHiragana('「') == false);
        Debug.Assert(Util.IsHiragana('」') == false);
        Debug.Assert(Util.IsHiragana('　') == false);
        Debug.Assert(Util.IsHiragana(' ') == false);

        Debug.Assert(Util.IsSpace('0') == false);
        Debug.Assert(Util.IsSpace('9') == false);
        Debug.Assert(Util.IsSpace('A') == false);
        Debug.Assert(Util.IsSpace('z') == false);
        Debug.Assert(Util.IsSpace('漢') == false);
        Debug.Assert(Util.IsSpace('０') == false);
        Debug.Assert(Util.IsSpace('９') == false);
        Debug.Assert(Util.IsSpace('ａ') == false);
        Debug.Assert(Util.IsSpace('Ｚ') == false);
        Debug.Assert(Util.IsSpace('あ') == false);
        Debug.Assert(Util.IsSpace('ゔ') == false);
        Debug.Assert(Util.IsSpace('ん') == false);
        Debug.Assert(Util.IsSpace('ば') == false);
        Debug.Assert(Util.IsSpace('ぱ') == false);
        Debug.Assert(Util.IsSpace('ー') == false);
        Debug.Assert(Util.IsSpace('ア') == false);
        Debug.Assert(Util.IsSpace('ン') == false);
        Debug.Assert(Util.IsSpace('ヴ') == false);
        Debug.Assert(Util.IsSpace('ｱ') == false);
        Debug.Assert(Util.IsSpace('ｳ') == false);
        Debug.Assert(Util.IsSpace('ﾝ') == false);
        Debug.Assert(Util.IsSpace('ー') == false);
        Debug.Assert(Util.IsSpace('-') == false);
        Debug.Assert(Util.IsSpace('!') == false);
        Debug.Assert(Util.IsSpace('*') == false);
        Debug.Assert(Util.IsSpace('－') == false);
        Debug.Assert(Util.IsSpace('！') == false);
        Debug.Assert(Util.IsSpace('＊') == false);
        Debug.Assert(Util.IsSpace('「') == false);
        Debug.Assert(Util.IsSpace('」') == false);
        Debug.Assert(Util.IsSpace('　') == true);
        Debug.Assert(Util.IsSpace(' ') == true);


        Debug.Assert(Util.IsAlpha('0') == false);
        Debug.Assert(Util.IsAlpha('9') == false);
        Debug.Assert(Util.IsAlpha('A') == true);
        Debug.Assert(Util.IsAlpha('z') == true);
        Debug.Assert(Util.IsAlpha('漢') == false);
        Debug.Assert(Util.IsAlpha('０') == false);
        Debug.Assert(Util.IsAlpha('９') == false);
        Debug.Assert(Util.IsAlpha('ａ') == true);
        Debug.Assert(Util.IsAlpha('Ｚ') == true);
        Debug.Assert(Util.IsAlpha('あ') == false);
        Debug.Assert(Util.IsAlpha('ゔ') == false);
        Debug.Assert(Util.IsAlpha('ん') == false);
        Debug.Assert(Util.IsAlpha('ば') == false);
        Debug.Assert(Util.IsAlpha('ぱ') == false);
        Debug.Assert(Util.IsAlpha('ー') == false);
        Debug.Assert(Util.IsAlpha('ア') == false);
        Debug.Assert(Util.IsAlpha('ン') == false);
        Debug.Assert(Util.IsAlpha('ヴ') == false);
        Debug.Assert(Util.IsAlpha('ｱ') == false);
        Debug.Assert(Util.IsAlpha('ｳ') == false);
        Debug.Assert(Util.IsAlpha('ﾝ') == false);
        Debug.Assert(Util.IsAlpha('ー') == false);
        Debug.Assert(Util.IsAlpha('-') == false);
        Debug.Assert(Util.IsAlpha('!') == false);
        Debug.Assert(Util.IsAlpha('*') == false);
        Debug.Assert(Util.IsAlpha('－') == false);
        Debug.Assert(Util.IsAlpha('！') == false);
        Debug.Assert(Util.IsAlpha('＊') == false);
        Debug.Assert(Util.IsAlpha('「') == false);
        Debug.Assert(Util.IsAlpha('」') == false);
        Debug.Assert(Util.IsAlpha('　') == false);
        Debug.Assert(Util.IsAlpha(' ') == false);


        Debug.Assert(Util.IsSymbol('0') == false);
        Debug.Assert(Util.IsSymbol('9') == false);
        Debug.Assert(Util.IsSymbol('A') == false);
        Debug.Assert(Util.IsSymbol('z') == false);
        Debug.Assert(Util.IsSymbol('漢') == false);
        Debug.Assert(Util.IsSymbol('０') == false);
        Debug.Assert(Util.IsSymbol('９') == false);
        Debug.Assert(Util.IsSymbol('ａ') == false);
        Debug.Assert(Util.IsSymbol('Ｚ') == false);
        Debug.Assert(Util.IsSymbol('あ') == false);
        Debug.Assert(Util.IsSymbol('ゔ') == false);
        Debug.Assert(Util.IsSymbol('ん') == false);
        Debug.Assert(Util.IsSymbol('ば') == false);
        Debug.Assert(Util.IsSymbol('ぱ') == false);
        Debug.Assert(Util.IsSymbol('ー') == false);
        Debug.Assert(Util.IsSymbol('ア') == false);
        Debug.Assert(Util.IsSymbol('ン') == false);
        Debug.Assert(Util.IsSymbol('ヴ') == false);
        Debug.Assert(Util.IsSymbol('ｱ') == false);
        Debug.Assert(Util.IsSymbol('ｳ') == false);
        Debug.Assert(Util.IsSymbol('ﾝ') == false);
        Debug.Assert(Util.IsSymbol('ー') == false);
        Debug.Assert(Util.IsSymbol('-') == true);
        Debug.Assert(Util.IsSymbol('!') == true);
        Debug.Assert(Util.IsSymbol('*') == true);
        Debug.Assert(Util.IsSymbol('－') == false);
        Debug.Assert(Util.IsSymbol('！') == false);
        Debug.Assert(Util.IsSymbol('＊') == false);
        Debug.Assert(Util.IsSymbol('「') == false);
        Debug.Assert(Util.IsSymbol('」') == false);
        Debug.Assert(Util.IsSymbol('　') == false);
        Debug.Assert(Util.IsSymbol(' ') == false);

        Debug.Assert(Util.IsSymbol('0', true) == false);
        Debug.Assert(Util.IsSymbol('9', true) == false);
        Debug.Assert(Util.IsSymbol('A', true) == false);
        Debug.Assert(Util.IsSymbol('z', true) == false);
        Debug.Assert(Util.IsSymbol('漢', true) == false);
        Debug.Assert(Util.IsSymbol('０', true) == false);
        Debug.Assert(Util.IsSymbol('９', true) == false);
        Debug.Assert(Util.IsSymbol('ａ', true) == false);
        Debug.Assert(Util.IsSymbol('Ｚ', true) == false);
        Debug.Assert(Util.IsSymbol('あ', true) == false);
        Debug.Assert(Util.IsSymbol('ゔ', true) == false);
        Debug.Assert(Util.IsSymbol('ん', true) == false);
        Debug.Assert(Util.IsSymbol('ば', true) == false);
        Debug.Assert(Util.IsSymbol('ぱ', true) == false);
        Debug.Assert(Util.IsSymbol('ー', true) == true);
        Debug.Assert(Util.IsSymbol('ア', true) == false);
        Debug.Assert(Util.IsSymbol('ン', true) == false);
        Debug.Assert(Util.IsSymbol('ヴ', true) == false);
        Debug.Assert(Util.IsSymbol('ｱ', true) == false);
        Debug.Assert(Util.IsSymbol('ｳ', true) == false);
        Debug.Assert(Util.IsSymbol('ﾝ', true) == false);
        Debug.Assert(Util.IsSymbol('ー', true) == true);
        Debug.Assert(Util.IsSymbol('-', true) == true);
        Debug.Assert(Util.IsSymbol('!', true) == true);
        Debug.Assert(Util.IsSymbol('*', true) == true);
        Debug.Assert(Util.IsSymbol('－', true) == true);
        Debug.Assert(Util.IsSymbol('！', true) == true);
        Debug.Assert(Util.IsSymbol('＊', true) == true);
        Debug.Assert(Util.IsSymbol('「', true) == true);
        Debug.Assert(Util.IsSymbol('」', true) == true);
        Debug.Assert(Util.IsSymbol('　', true) == false);
        Debug.Assert(Util.IsSymbol(' ', true) == false);


        Debug.Assert(Util.IsJPSymbol('0') == false);
        Debug.Assert(Util.IsJPSymbol('9') == false);
        Debug.Assert(Util.IsJPSymbol('A') == false);
        Debug.Assert(Util.IsJPSymbol('z') == false);
        Debug.Assert(Util.IsJPSymbol('漢') == false);
        Debug.Assert(Util.IsJPSymbol('０') == false);
        Debug.Assert(Util.IsJPSymbol('９') == false);
        Debug.Assert(Util.IsJPSymbol('ａ') == false);
        Debug.Assert(Util.IsJPSymbol('Ｚ') == false);
        Debug.Assert(Util.IsJPSymbol('あ') == false);
        Debug.Assert(Util.IsJPSymbol('ゔ') == false);
        Debug.Assert(Util.IsJPSymbol('ん') == false);
        Debug.Assert(Util.IsJPSymbol('ば') == false);
        Debug.Assert(Util.IsJPSymbol('ぱ') == false);
        Debug.Assert(Util.IsJPSymbol('ー') == true);
        Debug.Assert(Util.IsJPSymbol('ア') == false);
        Debug.Assert(Util.IsJPSymbol('ン') == false);
        Debug.Assert(Util.IsJPSymbol('ヴ') == false);
        Debug.Assert(Util.IsJPSymbol('ｱ') == false);
        Debug.Assert(Util.IsJPSymbol('ｳ') == false);
        Debug.Assert(Util.IsJPSymbol('ﾝ') == false);
        Debug.Assert(Util.IsJPSymbol('ー') == true);
        Debug.Assert(Util.IsJPSymbol('-') == false);
        Debug.Assert(Util.IsJPSymbol('!') == false);
        Debug.Assert(Util.IsJPSymbol('*') == false);
        Debug.Assert(Util.IsJPSymbol('－') == false);
        Debug.Assert(Util.IsJPSymbol('！') == false);
        Debug.Assert(Util.IsJPSymbol('＊') == false);
        Debug.Assert(Util.IsJPSymbol('「') == true);
        Debug.Assert(Util.IsJPSymbol('」') == true);
        Debug.Assert(Util.IsJPSymbol('　') == false);
        Debug.Assert(Util.IsJPSymbol(' ') == false);
    }

    /// <summary>CsvReadHelper テスト</summary>
    void TestCsvReadHelper() {
        Debug.Log("TestCsvReadHelper");

        TextAsset csv1 = new TextAsset();
        csv1 = Resources.Load("TypeModule/data/Char2Kana/roma", typeof(TextAsset)) as TextAsset;
        CsvReadHelper helper1 = new CsvReadHelper(csv1);

        Debug.Assert(helper1.FieldMax != 0);
        Debug.Assert(helper1.RecordNum != 0);
        string dummy = "";
        for (int i = 0; i < helper1.RecordNum; ++i){
            for (int j = 0; j < helper1.FieldMax; ++j){
                dummy = helper1.Datas[i][j];
            }
        }
        helper1.Load("TypeModule/data/KeyCode2Char/qwerty");
        for (int i = 0; i < helper1.RecordNum; ++i) {
            for (int j = 0; j < helper1.FieldMax; ++j) {
                dummy = helper1.Datas[i][j];
            }
        }
        helper1.Load(csv1);
        foreach(List<string> record in helper1.Datas){
            foreach(string d in record){
                dummy = d;
            }
        }

        CsvReadHelper helper2 = new CsvReadHelper("TypeModule/data/KeyCode2Char/qwerty");
        foreach (List<string> record in helper2.Datas) {
            foreach (string d in record) {
                dummy = d;
            }
        }
    }

    /// <summary>ConvertTableMgr テスト</summary>
    void TestConvertTableMgr() {
        Debug.Log("Test ConvertTableMgr");

        ConvertTableMgr cvt = new ConvertTableMgr();
        Debug.Assert(cvt.Key2Roma != null);
        Debug.Assert(cvt.Roma2Kana != null);
        Debug.Assert(cvt.Kana2Roma != null);
        Debug.Assert(cvt.Key2kanaMid != null);
        Debug.Assert(cvt.KanaMid2Kana != null);
        Debug.Assert(cvt.Kana2KanaMid != null);
        Debug.Assert(cvt.Key2Roma != null);

        cvt.EnabledCapsLock = true;
        cvt.EnabledCapsLock = false;
    }

    /// <summary>Kana2KanaMidTable テスト</summary>
    void TestKana2KanaMidTable() {
        Debug.Log("Test Kana2KanaMidTable");
        ConvertTableMgr cvt = new ConvertTableMgr();
        Kana2KanaMidTable table = cvt.Kana2KanaMid;

        string outCvt = "";
        Debug.Assert(table.TryConvert("あ", out outCvt) == false);
        Debug.Assert(table.TryConvert("ん", out outCvt) == false);
        Debug.Assert(table.TryConvert("が", out outCvt) == true);
        Debug.Assert(string.Compare(outCvt, "か゛") == 0);
        Debug.Assert(table.TryConvert("ぱ", out outCvt) == true);
        Debug.Assert(string.Compare(outCvt, "は゛") == 0);
        Debug.Assert(table.TryConvert("ぱ", out outCvt) == true);
        Debug.Assert(string.Compare(outCvt, "は゜") == 0);
        Debug.Assert(table.TryConvert("ゔ", out outCvt) == true);
        Debug.Assert(string.Compare(outCvt, "う゛") == 0);
        Debug.Assert(table.TryConvert("ぱば", out outCvt) == false);
        Debug.Assert(table.TryConvert("ゔあ", out outCvt) == false);

        Debug.Assert(table.TryConvert("0", out outCvt) == false);
        Debug.Assert(table.TryConvert("9", out outCvt) == false);
        Debug.Assert(table.TryConvert("A", out outCvt) == false);
        Debug.Assert(table.TryConvert("z", out outCvt) == false);
        Debug.Assert(table.TryConvert("漢", out outCvt) == false);
        Debug.Assert(table.TryConvert("０", out outCvt) == false);
        Debug.Assert(table.TryConvert("９", out outCvt) == false);
        Debug.Assert(table.TryConvert("ａ", out outCvt) == false);
        Debug.Assert(table.TryConvert("Ｚ", out outCvt) == false);
        Debug.Assert(table.TryConvert("ア", out outCvt) == false);
        Debug.Assert(table.TryConvert("ン", out outCvt) == false);
        Debug.Assert(table.TryConvert("ヴ", out outCvt) == false);
        Debug.Assert(table.TryConvert("ｱ", out outCvt) == false);
        Debug.Assert(table.TryConvert("ｳ", out outCvt) == false);
        Debug.Assert(table.TryConvert("ﾝ", out outCvt) == false);
        Debug.Assert(table.TryConvert("ー", out outCvt) == false);
        Debug.Assert(table.TryConvert("-", out outCvt) == false);
        Debug.Assert(table.TryConvert("!", out outCvt) == false);
        Debug.Assert(table.TryConvert("*", out outCvt) == false);
        Debug.Assert(table.TryConvert("－", out outCvt) == false);
        Debug.Assert(table.TryConvert("！", out outCvt) == false);
        Debug.Assert(table.TryConvert("＊", out outCvt) == false);
        Debug.Assert(table.TryConvert("「", out outCvt) == false);
        Debug.Assert(table.TryConvert("」", out outCvt) == false);
        Debug.Assert(table.TryConvert("　", out outCvt) == false);
        Debug.Assert(table.TryConvert(" ", out outCvt) == false);

        Debug.Assert(table.KanaMaxLength > 0);
        Debug.Assert(table.KanaMidMaxLength > 0);
    }

    /// <summary>Kana2RomaTable テスト</summary>
    void TestKana2RomaTable() {
        Debug.Log("Test Kana2RomaTable");
        ConvertTableMgr cvt = new ConvertTableMgr();
        Kana2RomaTable table = cvt.Kana2Roma;

        string outCvt = "";
        Debug.Assert(table.TryConvert("あ", out outCvt) == true);
        Debug.Assert(string.Compare(outCvt, "a") == 0);
        Debug.Assert(table.TryConvert("ん", out outCvt) == true);
        Debug.Assert(string.Compare(outCvt, "nn") == 0);
        Debug.Assert(table.TryConvert("ぱ", out outCvt) == true);
        Debug.Assert(string.Compare(outCvt, "pa") == 0);
        Debug.Assert(table.TryConvert("ゔ", out outCvt) == true);
        Debug.Assert(string.Compare(outCvt, "vu") == 0);
        Debug.Assert(table.TryConvert("ー", out outCvt) == false);
        Debug.Assert(table.TryConvert("は゛", out outCvt) == true);//TryGetValue内部で「ば」として判定するみたい
        Debug.Assert(string.Compare(outCvt, "ba") == 0);
        Debug.Assert(table.TryConvert("ばば", out outCvt) == false);
        Debug.Assert(table.TryConvert("ゔあ", out outCvt) == false);
        Debug.Assert(table.TryConvert("ぎゃ", out outCvt) == true);
        Debug.Assert(string.Compare(outCvt, "gya") == 0);

        Debug.Assert(table.TryConvert("0", out outCvt) == false);
        Debug.Assert(table.TryConvert("9", out outCvt) == false);
        Debug.Assert(table.TryConvert("A", out outCvt) == false);
        Debug.Assert(table.TryConvert("z", out outCvt) == false);
        Debug.Assert(table.TryConvert("漢", out outCvt) == false);
        Debug.Assert(table.TryConvert("０", out outCvt) == false);
        Debug.Assert(table.TryConvert("９", out outCvt) == false);
        Debug.Assert(table.TryConvert("ａ", out outCvt) == false);
        Debug.Assert(table.TryConvert("Ｚ", out outCvt) == false);
        Debug.Assert(table.TryConvert("ア", out outCvt) == false);
        Debug.Assert(table.TryConvert("ン", out outCvt) == false);
        Debug.Assert(table.TryConvert("ヴ", out outCvt) == false);
        Debug.Assert(table.TryConvert("ｱ", out outCvt) == false);
        Debug.Assert(table.TryConvert("ｳ", out outCvt) == false);
        Debug.Assert(table.TryConvert("ﾝ", out outCvt) == false);
        Debug.Assert(table.TryConvert("ー", out outCvt) == false);
        Debug.Assert(table.TryConvert("-", out outCvt) == false);
        Debug.Assert(table.TryConvert("!", out outCvt) == false);
        Debug.Assert(table.TryConvert("*", out outCvt) == false);
        Debug.Assert(table.TryConvert("－", out outCvt) == false);
        Debug.Assert(table.TryConvert("！", out outCvt) == false);
        Debug.Assert(table.TryConvert("＊", out outCvt) == false);
        Debug.Assert(table.TryConvert("「", out outCvt) == false);
        Debug.Assert(table.TryConvert("」", out outCvt) == false);
        Debug.Assert(table.TryConvert("　", out outCvt) == false);
        Debug.Assert(table.TryConvert(" ", out outCvt) == false);

        Debug.Assert(table.TryConvert("あ", out outCvt, "i") == false);
        Debug.Assert(table.TryConvert("ん", out outCvt, "n") == true);
        Debug.Assert(string.Compare(outCvt, "nn") == 0);
        Debug.Assert(table.TryConvert("ん", out outCvt, "xn") == true);
        Debug.Assert(string.Compare(outCvt, "xn") == 0);
        Debug.Assert(table.TryConvert("い", out outCvt, "i") == true);
        Debug.Assert(string.Compare(outCvt, "i") == 0);
        Debug.Assert(table.TryConvert("ゔ", out outCvt, "va") == false);
        Debug.Assert(table.TryConvert("ちゃ", out outCvt, "tya") == true);
        Debug.Assert(string.Compare(outCvt, "tya") == 0);
        Debug.Assert(table.TryConvert("ちゃ", out outCvt, "t") == true);
        Debug.Assert(string.Compare(outCvt, "tya") == 0);
        Debug.Assert(table.TryConvert("ちゃ", out outCvt, "c") == true);
        Debug.Assert(string.Compare(outCvt, "cha") == 0);
        Debug.Assert(table.TryConvert("ばば", out outCvt, "a") == false);
        Debug.Assert(table.TryConvert("ゔあ", out outCvt, "a") == false);
        Debug.Assert(table.TryConvert("ぎゃ", out outCvt, "a") == false);

        Debug.Assert(table.TryConvert("0", out outCvt, "a") == false);
        Debug.Assert(table.TryConvert("9", out outCvt, "a") == false);
        Debug.Assert(table.TryConvert("A", out outCvt, "a") == false);
        Debug.Assert(table.TryConvert("z", out outCvt, "a") == false);
        Debug.Assert(table.TryConvert("漢", out outCvt, "hoge") == false);
        Debug.Assert(table.TryConvert("０", out outCvt, "hoge") == false);
        Debug.Assert(table.TryConvert("９", out outCvt, "hoge") == false);
        Debug.Assert(table.TryConvert("ａ", out outCvt, "hoge") == false);
        Debug.Assert(table.TryConvert("Ｚ", out outCvt, "hoge") == false);
        Debug.Assert(table.TryConvert("ア", out outCvt, "hoge") == false);
        Debug.Assert(table.TryConvert("ン", out outCvt, "hoge") == false);
        Debug.Assert(table.TryConvert("ヴ", out outCvt, "hoge") == false);
        Debug.Assert(table.TryConvert("ｱ", out outCvt, "1") == false);
        Debug.Assert(table.TryConvert("ｳ", out outCvt, "1") == false);
        Debug.Assert(table.TryConvert("ﾝ", out outCvt, "1") == false);
        Debug.Assert(table.TryConvert("ー", out outCvt, "3") == false);
        Debug.Assert(table.TryConvert("-", out outCvt, "あ") == false);
        Debug.Assert(table.TryConvert("!", out outCvt, "あ") == false);
        Debug.Assert(table.TryConvert("*", out outCvt, "あ") == false);
        Debug.Assert(table.TryConvert("－", out outCvt, "ch") == false);
        Debug.Assert(table.TryConvert("！", out outCvt, "ch") == false);
        Debug.Assert(table.TryConvert("＊", out outCvt, "ch") == false);
        Debug.Assert(table.TryConvert("「", out outCvt, "ch") == false);
        Debug.Assert(table.TryConvert("」", out outCvt, "ch") == false);
        Debug.Assert(table.TryConvert("　", out outCvt, "ch") == false);
        Debug.Assert(table.TryConvert(" ", out outCvt) == false);

        Debug.Assert(table.KanaMaxLength > 0);
    }

    /// <summary>Kana2RomaTable テスト</summary>
    void TestKanaMid2KanaTable() {
        Debug.Log("Test KanaMid2KanaTable");
        ConvertTableMgr cvt = new ConvertTableMgr();
        KanaMid2KanaTable table = cvt.KanaMid2Kana;

        string outCvt = "";
        Debug.Assert(table.TryConvert("あ", out outCvt) == false);
        Debug.Assert(table.TryConvert("ん", out outCvt) == false);
        Debug.Assert(table.TryConvert("ん", out outCvt) == false);
        Debug.Assert(table.TryConvert("は", out outCvt) == false);
        Debug.Assert(table.TryConvert("ば", out outCvt) == false);
        Debug.Assert(table.TryConvert("ぱ", out outCvt) == false);
        Debug.Assert(table.TryConvert("ゔ", out outCvt) == false);
        Debug.Assert(table.TryConvert("ー", out outCvt) == false);
        Debug.Assert(table.TryConvert("は゛", out outCvt) == true);
        Debug.Assert(string.Compare(outCvt, "ば") == 0);
        Debug.Assert(table.TryConvert("は゜", out outCvt) == true);
        Debug.Assert(string.Compare(outCvt, "ぱ") == 0);
        Debug.Assert(table.TryConvert("う゛", out outCvt) == true);
        Debug.Assert(string.Compare(outCvt, "ゔ") == 0);
        Debug.Assert(table.TryConvert("は゛ふ", out outCvt) == false);
        Debug.Assert(table.TryConvert("はあ゜", out outCvt) == false);
        Debug.Assert(table.TryConvert("゛う゛", out outCvt) == false);

        Debug.Assert(table.TryConvert("ばば", out outCvt) == false);
        Debug.Assert(table.TryConvert("ゔあ", out outCvt) == false);
        Debug.Assert(table.TryConvert("ぎゃ", out outCvt) == false);

        Debug.Assert(table.TryConvert("0", out outCvt) == false);
        Debug.Assert(table.TryConvert("9", out outCvt) == false);
        Debug.Assert(table.TryConvert("A", out outCvt) == false);
        Debug.Assert(table.TryConvert("z", out outCvt) == false);
        Debug.Assert(table.TryConvert("漢", out outCvt) == false);
        Debug.Assert(table.TryConvert("０", out outCvt) == false);
        Debug.Assert(table.TryConvert("９", out outCvt) == false);
        Debug.Assert(table.TryConvert("ａ", out outCvt) == false);
        Debug.Assert(table.TryConvert("Ｚ", out outCvt) == false);
        Debug.Assert(table.TryConvert("ア", out outCvt) == false);
        Debug.Assert(table.TryConvert("ン", out outCvt) == false);
        Debug.Assert(table.TryConvert("ヴ", out outCvt) == false);
        Debug.Assert(table.TryConvert("ｱ", out outCvt) == false);
        Debug.Assert(table.TryConvert("ｳ", out outCvt) == false);
        Debug.Assert(table.TryConvert("ﾝ", out outCvt) == false);
        Debug.Assert(table.TryConvert("ー", out outCvt) == false);
        Debug.Assert(table.TryConvert("-", out outCvt) == false);
        Debug.Assert(table.TryConvert("!", out outCvt) == false);
        Debug.Assert(table.TryConvert("*", out outCvt) == false);
        Debug.Assert(table.TryConvert("－", out outCvt) == false);
        Debug.Assert(table.TryConvert("！", out outCvt) == false);
        Debug.Assert(table.TryConvert("＊", out outCvt) == false);
        Debug.Assert(table.TryConvert("「", out outCvt) == false);
        Debug.Assert(table.TryConvert("」", out outCvt) == false);
        Debug.Assert(table.TryConvert("　", out outCvt) == false);
        Debug.Assert(table.TryConvert(" ", out outCvt) == false);


        Debug.Assert(table.TryConvert("あ", out outCvt, true) == false);
        Debug.Assert(table.TryConvert("ん", out outCvt, true) == false);
        Debug.Assert(table.TryConvert("ん", out outCvt, true) == false);
        Debug.Assert(table.TryConvert("は", out outCvt, true) == true);
        Debug.Assert(string.Compare(outCvt, "ば") == 0);
        Debug.Assert(table.TryConvert("ば", out outCvt, true) == false);
        Debug.Assert(table.TryConvert("ぱ", out outCvt, true) == false);
        Debug.Assert(table.TryConvert("ゔ", out outCvt, true) == false);
        Debug.Assert(table.TryConvert("ー", out outCvt, true) == false);
        Debug.Assert(table.TryConvert("は゛", out outCvt, true) == true);
        Debug.Assert(string.Compare(outCvt, "ば") == 0);
        Debug.Assert(table.TryConvert("は゜", out outCvt, true) == true);
        Debug.Assert(string.Compare(outCvt, "ぱ") == 0);    
        Debug.Assert(table.TryConvert("う゛", out outCvt, true) == true);
        Debug.Assert(string.Compare(outCvt, "ゔ") == 0);
        Debug.Assert(table.TryConvert("は゛ふ", out outCvt, true) == false);
        Debug.Assert(table.TryConvert("はあ゜", out outCvt, true) == false);
        Debug.Assert(table.TryConvert("゛う゛", out outCvt, true) == false);

        Debug.Assert(table.TryConvert("0", out outCvt, true) == false);
        Debug.Assert(table.TryConvert("9", out outCvt, true) == false);
        Debug.Assert(table.TryConvert("A", out outCvt, true) == false);
        Debug.Assert(table.TryConvert("z", out outCvt, true) == false);
        Debug.Assert(table.TryConvert("漢", out outCvt, true) == false);
        Debug.Assert(table.TryConvert("０", out outCvt, true) == false);
        Debug.Assert(table.TryConvert("９", out outCvt, true) == false);
        Debug.Assert(table.TryConvert("ａ", out outCvt, true) == false);
        Debug.Assert(table.TryConvert("Ｚ", out outCvt, true) == false);
        Debug.Assert(table.TryConvert("ア", out outCvt, true) == false);
        Debug.Assert(table.TryConvert("ン", out outCvt, true) == false);
        Debug.Assert(table.TryConvert("ヴ", out outCvt, true) == false);
        Debug.Assert(table.TryConvert("ｱ", out outCvt, true) == false);
        Debug.Assert(table.TryConvert("ｳ", out outCvt, true) == false);
        Debug.Assert(table.TryConvert("ﾝ", out outCvt, true) == false);
        Debug.Assert(table.TryConvert("ー", out outCvt, true) == false);
        Debug.Assert(table.TryConvert("-", out outCvt, true) == false);
        Debug.Assert(table.TryConvert("!", out outCvt, true) == false);
        Debug.Assert(table.TryConvert("*", out outCvt, true) == false);
        Debug.Assert(table.TryConvert("－", out outCvt, true) == false);
        Debug.Assert(table.TryConvert("！", out outCvt, true) == false);
        Debug.Assert(table.TryConvert("＊", out outCvt, true) == false);
        Debug.Assert(table.TryConvert("「", out outCvt, true) == false);
        Debug.Assert(table.TryConvert("」", out outCvt, true) == false);
        Debug.Assert(table.TryConvert("　", out outCvt, true) == false);
        Debug.Assert(table.TryConvert(" ", out outCvt, true) == false);

        Debug.Assert(table.KanaMidMaxLength > 0);
    }

    /// <summary>KeyCode2CharTable テスト</summary>
    void TestKeyCode2CharTable() {
        Debug.Log("Test KeyCode2CharTable");
        ConvertTableMgr cvt = new ConvertTableMgr();
        
        KeyCode2CharTable table = cvt.Key2Roma;

        Debug.Assert(table.Convert(KeyCode.Alpha0, false , false) == '0');
        Debug.Assert(table.Convert(KeyCode.Alpha0, true, false) == '\0');
        Debug.Assert(table.Convert(KeyCode.Alpha0, false, true) == '\0');
        Debug.Assert(table.Convert(KeyCode.Alpha0, true, true) == '\0');
        Debug.Assert(table.Convert(KeyCode.Alpha1, false, false) == '1');
        Debug.Assert(table.Convert(KeyCode.Alpha1, true, false) == '!');
        Debug.Assert(table.Convert(KeyCode.Alpha1, false, true) == '\0');
        Debug.Assert(table.Convert(KeyCode.Alpha1, true, true) == '\0');
        Debug.Assert(table.Convert(KeyCode.Alpha9, false, false) == '9');
        Debug.Assert(table.Convert(KeyCode.Alpha9, true, false) == ')');
        Debug.Assert(table.Convert(KeyCode.Alpha9, false, true) == '\0');
        Debug.Assert(table.Convert(KeyCode.Alpha9, true, true) == '\0');
        Debug.Assert(table.Convert(KeyCode.A, false, false) == 'a');
        Debug.Assert(table.Convert(KeyCode.A, true, false) == 'A');
        Debug.Assert(table.Convert(KeyCode.A, false, true) == '\0');
        Debug.Assert(table.Convert(KeyCode.A, true, true) == '\0');
        Debug.Assert(table.Convert(KeyCode.Z, false, false) == 'z');
        Debug.Assert(table.Convert(KeyCode.Z, true, false) == 'Z');
        Debug.Assert(table.Convert(KeyCode.Z, false, true) == '\0');
        Debug.Assert(table.Convert(KeyCode.Z, true, true) == '\0');
        Debug.Assert(table.Convert(KeyCode.Backslash, false, false) == '\\');
        Debug.Assert(table.Convert(KeyCode.Backslash, true, false) == '|');
        Debug.Assert(table.Convert(KeyCode.Backslash, false, true) == '\\');
        Debug.Assert(table.Convert(KeyCode.Backslash, true, true) == '_');
        Debug.Assert(table.Convert(KeyCode.Comma, false, false) == ',');
        Debug.Assert(table.Convert(KeyCode.Comma, true, false) == '<');
        Debug.Assert(table.Convert(KeyCode.Comma, false, true) == '\0');
        Debug.Assert(table.Convert(KeyCode.Comma, true, true) == '\0');
        Debug.Assert(table.Convert(KeyCode.Return, false, false) == '\0');
        Debug.Assert(table.Convert(KeyCode.Return, true, false) == '\0');
        Debug.Assert(table.Convert(KeyCode.Return, false, true) == '\0');
        Debug.Assert(table.Convert(KeyCode.Return, true, true) == '\0');
        Debug.Assert(table.Convert(KeyCode.Backspace, false, false) == '\0');
        Debug.Assert(table.Convert(KeyCode.Backspace, true, false) == '\0');
        Debug.Assert(table.Convert(KeyCode.Backspace, false, true) == '\0');
        Debug.Assert(table.Convert(KeyCode.Backspace, true, true) == '\0');
        Debug.Assert(table.Convert(KeyCode.DownArrow, false, false) == '\0');
        Debug.Assert(table.Convert(KeyCode.DownArrow, true, false) == '\0');
        Debug.Assert(table.Convert(KeyCode.DownArrow, false, true) == '\0');
        Debug.Assert(table.Convert(KeyCode.DownArrow, true, true) == '\0');

        table = cvt.Key2kanaMid;

        Debug.Assert(table.Convert(KeyCode.Alpha0, false, false) == 'わ');
        Debug.Assert(table.Convert(KeyCode.Alpha0, true, false) == 'を');
        Debug.Assert(table.Convert(KeyCode.Alpha0, false, true) == '\0');
        Debug.Assert(table.Convert(KeyCode.Alpha0, true, true) == '\0');
        Debug.Assert(table.Convert(KeyCode.Alpha1, false, false) == 'ぬ');
        Debug.Assert(table.Convert(KeyCode.Alpha1, true, false) == 'ぬ');
        Debug.Assert(table.Convert(KeyCode.Alpha1, false, true) == '\0');
        Debug.Assert(table.Convert(KeyCode.Alpha1, true, true) == '\0');
        Debug.Assert(table.Convert(KeyCode.Alpha9, false, false) == 'よ');
        Debug.Assert(table.Convert(KeyCode.Alpha9, true, false) == 'ょ');
        Debug.Assert(table.Convert(KeyCode.Alpha9, false, true) == '\0');
        Debug.Assert(table.Convert(KeyCode.Alpha9, true, true) == '\0');
        Debug.Assert(table.Convert(KeyCode.A, false, false) == 'ち');
        Debug.Assert(table.Convert(KeyCode.A, true, false) == 'ち');
        Debug.Assert(table.Convert(KeyCode.A, false, true) == '\0');
        Debug.Assert(table.Convert(KeyCode.A, true, true) == '\0');
        Debug.Assert(table.Convert(KeyCode.Z, false, false) == 'つ');
        Debug.Assert(table.Convert(KeyCode.Z, true, false) == 'っ');
        Debug.Assert(table.Convert(KeyCode.Z, false, true) == '\0');
        Debug.Assert(table.Convert(KeyCode.Z, true, true) == '\0');
        Debug.Assert(table.Convert(KeyCode.Backslash, false, false) == 'ー');
        Debug.Assert(table.Convert(KeyCode.Backslash, true, false) == 'ー');
        Debug.Assert(table.Convert(KeyCode.Backslash, false, true) == 'ろ');
        Debug.Assert(table.Convert(KeyCode.Backslash, true, true) == 'ろ');
        Debug.Assert(table.Convert(KeyCode.Comma, false, false) == 'ね');
        Debug.Assert(table.Convert(KeyCode.Comma, true, false) == '、');
        Debug.Assert(table.Convert(KeyCode.Comma, false, true) == '\0');
        Debug.Assert(table.Convert(KeyCode.Comma, true, true) == '\0');
        Debug.Assert(table.Convert(KeyCode.Return, false, false) == '\0');
        Debug.Assert(table.Convert(KeyCode.Return, true, false) == '\0');
        Debug.Assert(table.Convert(KeyCode.Return, false, true) == '\0');
        Debug.Assert(table.Convert(KeyCode.Return, true, true) == '\0');
        Debug.Assert(table.Convert(KeyCode.Backspace, false, false) == '\0');
        Debug.Assert(table.Convert(KeyCode.Backspace, true, false) == '\0');
        Debug.Assert(table.Convert(KeyCode.Backspace, false, true) == '\0');
        Debug.Assert(table.Convert(KeyCode.Backspace, true, true) == '\0');
        Debug.Assert(table.Convert(KeyCode.DownArrow, false, false) == '\0');
        Debug.Assert(table.Convert(KeyCode.DownArrow, true, false) == '\0');
        Debug.Assert(table.Convert(KeyCode.DownArrow, false, true) == '\0');
        Debug.Assert(table.Convert(KeyCode.DownArrow, true, true) == '\0');
    }

    /// <summary>KeyCode2CharTable テスト</summary>
    void TestRoma2KanaTable() {
        Debug.Log("Test Roma2KanaTable");
        ConvertTableMgr cvt = new ConvertTableMgr();

        Roma2KanaTable table = cvt.Roma2Kana;
        string outCvt = "";
        Debug.Assert(table.TryConvert("a", out outCvt) == true);
        Debug.Assert(string.Compare(outCvt, "あ") == 0);
        Debug.Assert(table.TryConvert("nn", out outCvt) == true);
        Debug.Assert(string.Compare(outCvt, "ん") == 0);
        Debug.Assert(table.TryConvert("xn", out outCvt) == true);
        Debug.Assert(string.Compare(outCvt, "ん") == 0);
        Debug.Assert(table.TryConvert("x", out outCvt) == false);
        Debug.Assert(table.TryConvert("ch", out outCvt) == false);
        Debug.Assert(table.TryConvert("vu", out outCvt) == true);
        Debug.Assert(string.Compare(outCvt, "ゔ") == 0);
        Debug.Assert(table.TryConvert("aiu", out outCvt) == false);
        Debug.Assert(table.TryConvert("chm", out outCvt) == false);


        Debug.Assert(table.TryConvert("あ", out outCvt) == false);
        Debug.Assert(table.TryConvert("ん", out outCvt) == false);
        Debug.Assert(table.TryConvert("ん", out outCvt) == false);
        Debug.Assert(table.TryConvert("は", out outCvt) == false);
        Debug.Assert(table.TryConvert("ば", out outCvt) == false);
        Debug.Assert(table.TryConvert("ぱ", out outCvt) == false);
        Debug.Assert(table.TryConvert("ゔ", out outCvt) == false);
        Debug.Assert(table.TryConvert("ー", out outCvt) == false);
        Debug.Assert(table.TryConvert("は゛", out outCvt) == false);
        Debug.Assert(table.TryConvert("は゜", out outCvt) == false);
        Debug.Assert(table.TryConvert("う゛", out outCvt) == false);
        Debug.Assert(table.TryConvert("は゛ふ", out outCvt) == false);
        Debug.Assert(table.TryConvert("はあ゜", out outCvt) == false);
        Debug.Assert(table.TryConvert("゛う゛", out outCvt) == false);

        Debug.Assert(table.TryConvert("ばば", out outCvt) == false);
        Debug.Assert(table.TryConvert("ゔあ", out outCvt) == false);
        Debug.Assert(table.TryConvert("ぎゃ", out outCvt) == false);

        Debug.Assert(table.TryConvert("0", out outCvt) == false);
        Debug.Assert(table.TryConvert("9", out outCvt) == false);
        Debug.Assert(table.TryConvert("A", out outCvt) == true);
        Debug.Assert(string.Compare(outCvt, "あ") == 0);
        Debug.Assert(table.TryConvert("z", out outCvt) == false);
        Debug.Assert(table.TryConvert("漢", out outCvt) == false);
        Debug.Assert(table.TryConvert("０", out outCvt) == false);
        Debug.Assert(table.TryConvert("９", out outCvt) == false);
        Debug.Assert(table.TryConvert("ａ", out outCvt) == true);
        Debug.Assert(string.Compare(outCvt, "あ") == 0);
        Debug.Assert(table.TryConvert("Ｚ", out outCvt) == false);
        Debug.Assert(table.TryConvert("ア", out outCvt) == false);
        Debug.Assert(table.TryConvert("ン", out outCvt) == false);
        Debug.Assert(table.TryConvert("ヴ", out outCvt) == false);
        Debug.Assert(table.TryConvert("ｱ", out outCvt) == false);
        Debug.Assert(table.TryConvert("ｳ", out outCvt) == false);
        Debug.Assert(table.TryConvert("ﾝ", out outCvt) == false);
        Debug.Assert(table.TryConvert("ー", out outCvt) == false);
        Debug.Assert(table.TryConvert("-", out outCvt) == false);
        Debug.Assert(table.TryConvert("!", out outCvt) == false);
        Debug.Assert(table.TryConvert("*", out outCvt) == false);
        Debug.Assert(table.TryConvert("－", out outCvt) == false);
        Debug.Assert(table.TryConvert("！", out outCvt) == false);
        Debug.Assert(table.TryConvert("＊", out outCvt) == false);
        Debug.Assert(table.TryConvert("「", out outCvt) == false);
        Debug.Assert(table.TryConvert("」", out outCvt) == false);
        Debug.Assert(table.TryConvert("　", out outCvt) == false);
        Debug.Assert(table.TryConvert(" ", out outCvt) == false);


        Debug.Assert(table.HasPossibility("a") == false);
        Debug.Assert(table.HasPossibility("nn") == false);
        Debug.Assert(table.HasPossibility("xn") == false);
        Debug.Assert(table.HasPossibility("x") == true);
        Debug.Assert(table.HasPossibility("ch") == true);
        Debug.Assert(table.HasPossibility("vu") == false);
        Debug.Assert(table.HasPossibility("aiu") == false);
        Debug.Assert(table.HasPossibility("chm") == false);

        Debug.Assert(table.HasPossibility("あ") == false);
        Debug.Assert(table.HasPossibility("ん") == false);
        Debug.Assert(table.HasPossibility("ん") == false);
        Debug.Assert(table.HasPossibility("は") == false);
        Debug.Assert(table.HasPossibility("ば") == false);
        Debug.Assert(table.HasPossibility("ぱ") == false);
        Debug.Assert(table.HasPossibility("ゔ") == false);
        Debug.Assert(table.HasPossibility("ー") == false);
        Debug.Assert(table.HasPossibility("は゛") == false);
        Debug.Assert(table.HasPossibility("は゜") == false);
        Debug.Assert(table.HasPossibility("う゛") == false);
        Debug.Assert(table.HasPossibility("は゛ふ") == false);
        Debug.Assert(table.HasPossibility("はあ゜") == false);
        Debug.Assert(table.HasPossibility("゛う゛") == false);

        Debug.Assert(table.HasPossibility("ばば") == false);
        Debug.Assert(table.HasPossibility("ゔあ") == false);
        Debug.Assert(table.HasPossibility("ぎゃ") == false);

        Debug.Assert(table.HasPossibility("0") == false);
        Debug.Assert(table.HasPossibility("9") == false);
        Debug.Assert(table.HasPossibility("A") == false);
        Debug.Assert(table.HasPossibility("z") == true);
        Debug.Assert(table.HasPossibility("漢") == false);
        Debug.Assert(table.HasPossibility("０") == false);
        Debug.Assert(table.HasPossibility("９") == false);
        Debug.Assert(table.HasPossibility("ａ") == false);
        Debug.Assert(table.HasPossibility("Ｚ") == true);
        Debug.Assert(table.HasPossibility("ア") == false);
        Debug.Assert(table.HasPossibility("ン") == false);
        Debug.Assert(table.HasPossibility("ヴ") == false);
        Debug.Assert(table.HasPossibility("ｱ") == false);
        Debug.Assert(table.HasPossibility("ｳ") == false);
        Debug.Assert(table.HasPossibility("ﾝ") == false);
        Debug.Assert(table.HasPossibility("ー") == false);
        Debug.Assert(table.HasPossibility("-") == false);
        Debug.Assert(table.HasPossibility("!") == false);
        Debug.Assert(table.HasPossibility("*") == false);
        Debug.Assert(table.HasPossibility("－") == false);
        Debug.Assert(table.HasPossibility("！") == false);
        Debug.Assert(table.HasPossibility("＊") == false);
        Debug.Assert(table.HasPossibility("「") == false);
        Debug.Assert(table.HasPossibility("」") == false);
        Debug.Assert(table.HasPossibility("　") == false);
        Debug.Assert(table.HasPossibility(" ") == false);


        Debug.Assert(table.HasPossibility("a","あ") == false);
        Debug.Assert(table.HasPossibility("nn", "ん") == false);
        Debug.Assert(table.HasPossibility("xn", "つ") == false);
        Debug.Assert(table.HasPossibility("x", "ん") == true);
        Debug.Assert(table.HasPossibility("x", "んい") == false);
        Debug.Assert(table.HasPossibility("x", "あ") == false);
        Debug.Assert(table.HasPossibility("ch", "ちゃ") == true);
        Debug.Assert(table.HasPossibility("vu", "ゔ") == false);
        Debug.Assert(table.HasPossibility("aiu", "う") == false);
        Debug.Assert(table.HasPossibility("chm", "つ") == false);


        Debug.Assert(table.GetPossibilityKanas("a").Count == 0);
        Debug.Assert(table.GetPossibilityKanas("nn").Count == 0);
        Debug.Assert(table.GetPossibilityKanas("xn").Count == 0);
        Debug.Assert(table.GetPossibilityKanas("x").Count > 0);
        Debug.Assert(table.GetPossibilityKanas("t").Count > 0);
        Debug.Assert(table.GetPossibilityKanas("l").Count > 0);
        Debug.Assert(table.GetPossibilityKanas("ch").Count > 0);
        Debug.Assert(table.GetPossibilityKanas("vu").Count == 0);
        Debug.Assert(table.GetPossibilityKanas("aiu").Count == 0);
        Debug.Assert(table.GetPossibilityKanas("chm").Count == 0);
    }
    #endregion


    #region インスペクターパラメータ
    [Tooltip("テストを行うかどうかのフラグ")]
    #endregion
    /// <summary>テストを行うかどうかのフラグ</summary>
    public bool IsTest = true;

}
