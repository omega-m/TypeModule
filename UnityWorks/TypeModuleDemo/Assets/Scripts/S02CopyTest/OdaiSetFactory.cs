using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 関数ポインタ作成用
/// </summary>
public delegate List<string> OdayFuncInner();
public struct OdayFunction {
    public OdayFunction(OdayFuncInner aFunc) {
        m_func = aFunc;
    }
    public OdayFuncInner m_func;
}

/// <summary>02CopyTest用の、タイピングする文章セットのファクトリ</summary>
static public class OdaiSetFactory{

    #region 静的メソッド
    /// <summary>
    /// <para>02CopyTest用の、タイピングする文章セットを作成し返却します。</para>
    /// <para>呼び出す度に、ランダムな文章セットが返却されます。</para>
    /// </summary>
    /// <returns></returns>
    static public List<string> Create() {
        OdayFunction[] odaiList = {
            new OdayFunction(CreatePhony),
            new OdayFunction(CreateGoodbyeDeclaration),
            new OdayFunction(CreateVampire),
        };

        int idx = (int)Mathf.Floor(Random.value * odaiList.Length);
        return odaiList[idx].m_func();
    }
    #endregion

    #region 内部メソッド
    /// <summary>フォニイ</summary>
    /// <returns>「フォニイ」の歌詞セット</returns>
    static private List<string> CreatePhony() {
            return new List<string>() {
            "このよでぞうかよりきれいなはなはないわ",
            "なぜならばすべてはうそでできている",
            "antipathy world",
            "ぜつぼうのあめはあたしのかさをついて",
            "しめらすまえがみとこころのりめん",
            "わずらわしいわ",
            "いつしかことのははとうにかれきって",
            "ことのみがあたしにうれている",
            "かがみにうつりうそをえがいて",
            "みずからをみうしなっためいく",
            "パパッパラパッパララッパッパ",
            "なぞなぞかぞえてあそびましょう",
            "タタッタラタッタララッタッタ",
            "なぜなぜここでおどっているでしょう",
            "かんたんなこともわからないわ",
            "あたしってなんだっけ",
            "それすらよるのてにほだされて",
            "あいのようにきえる",
            "さようならもいえぬままないたフォニイ",
            "うそにからまっているあたしはフォニイ",
            "antipathy world",
        };
    }

    /// <summary>「グッバイ宣言」</summary>
    /// <returns>「グッバイ宣言」の歌詞セット</returns>
    static private List<string> CreateGoodbyeDeclaration() {
        return new List<string>() {
            "エマージェンシーれいじやつらは",
            "クレイジーインザタウンうちにこもって",
            "ゴロゴロゴロゴロと",
            "だらくのよるにからみついた",
            "ルルルはなつことばは",
            "ルルルくさっていた",
            "せいろんもじょうしきも",
            "いみをもたないとかいにサヨウナラ",
            "ひきこもりぜったいジャスティス",
            "おれのわたしだけのおりのなかで",
            "ききころしてランデブー",
            "おれのわたしのねがきみにそまるまで",
            "ひきこもりぜったいジャスティス",
            "おれのわたしだけのおりのなかで",
            "ききころしてランデブー",
            "おれのわたしのねをきみがつつむだけ",
            "wowow ときがきたいま",
            "wowow エゴはなつのさ",
            "wowow うちにこもってくるいざく",
        };
    }

    /// <summary>「ヴァンパイア」</summary>
    /// <returns>「ヴァンパイア」の歌詞セット</returns>
    static private List<string> CreateVampire() {
        return new List<string>() {
            "あたしヴァンパイア",
            "いいの？すっちゃっていいの？",
            "「もうむりもうむり」なんてわるいこだね",
            "ためしたいないっぱいではきたい",
            "まだぜったいいけるよ",
            "さいていさいこうずっといききしてる",
            "あまくなるふあんのかじつ",
            "No more はってんしっといきをしても",
            "いらないだけうるさいだけ",
            "だれかといればそれはたられば",
            "つよがってたってきもちにゃさからえない",
            "はなれていてもかんじてるエモ",
            "つながってたしかめたらしねるかも",
            "いいもんかなしいもんせつないもん",
            "きみのすべてをくらうまでぜっきょう",
            "あたしヴァンパイア",
            "いいの？すっちゃっていいの？",
            "「もうむりもうむり」なんてわるいこだね",
            "ためしたいないっぱいではきたい",
            "まだぜったいいけるよ",
            "あたしヴァンパイア",
            "もとめちゃってまたからしちゃってほらやなかんじ",
            "ないてわすれたら「はじめまして」",
            "あたしヴァンパイア",
            "あいじょうをくださいまだぜったいいけるよ",
            "あたしヴァンパイアまずはこっちおいで",
        };
    }

    #endregion
}
