using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text.RegularExpressions;

namespace TypeModule {
namespace Inner {

        #region メモ
        // 木構造にするのではなく、SortedDictionaryを利用した方がいい？
        //
        // PossibilityKanasに関しては、見つかった地点から下方向に向けて、一致している数を事前にチェックし、そのぶんのデータを返却するだけで終わりそう
        // しかし、毎回List<string>()として作成し返却すると重いし、かと言って事前に生成しておくと結局使用するメモリ量は同じ位 => list.GetRange() を使うと参照コピーになるみたい　よってメモリ量的には SortedDictionary のがかなりいい
        //
        // Convert()   のアクセス速度が n から log2(n)
        // 頻度が多そうなCanConvert()は　どちらもlog2(n) しかし、木構造のが早期リターンが多い
        //
        // とりあえず木構造で実装しておいて、メモリ的な不安が多そうなら書き変えることにします。
        #endregion

        /// <summary>ローマ字列からひらがな文字列に変換する為のテーブルを管理するクラスです。</summary>
        /// <example><code>
        /// using Inner;
        /// 
        ///     ...
        ///     
        /// //初期化処理
        /// Roma2KanaTable table = new Roma2KanaTable(in csvSrc);
        /// 
        /// 
        /// //ローマ字文字列からひらがな文字列へ変換
        /// string roma1 = "kya";
        /// string kana1 = table.Convert(roma1);
        /// if(kana1.Length &gt; 0){
        ///     Debug.Log(kana1);    // "きゃ"
        /// }
        /// 
        /// 
        /// //ローマ字文字列から、変換できるひらがながあるかをチェックおよび取得
        /// Debug.Log(table.TryConvert("a"));             // false
        /// Debug.Log(table.TryConvert("sh"));            // false
        /// Debug.Log(table.TryConvert("shi"));           // true
        /// 
        /// 
        /// //ローマ字文字列に追加でローマ字を足すことで、ひらがな文字列に変換する事が可能か
        /// string roma2 = "ty";
        /// string kana2 = "ちゅ";
        /// Debug.Log(table.HasPossibility(roma2, kana2));           // "false"
        /// //将来打てる文字が1つでもあればtrue
        /// Debug.Log(table.HasPossibility(roma2,));     // "true"
        /// 
        /// 
        /// //将来打てる可能性があるひらがな文字列一覧を取得
        /// List&lt;string&gt; kanaList1 = table.GetPossibilityKanas("tya");
        /// foreach(string k in kanaList1){
        ///     Debug.Log(k);                                   // none
        /// }
        /// 
        /// 
        /// //[n]1回で[ん]に変換する事ができるか
        /// Debug.Log(Roma2KanaTable.CanConverFirstN("n"));         //false
        /// Debug.Log(Roma2KanaTable.CanConverFirstN("na"));        //false
        /// Debug.Log(Roma2KanaTable.CanConverFirstN("nt"));        //true
        /// Debug.Log(Roma2KanaTable.CanConverFirstN("any"));       //false
        /// Debug.Log(Roma2KanaTable.CanConverFirstN("nn"));        //false
        /// 
        /// </code></example>
        public class Roma2KanaTable{


        # region 生成
        ///<summary>ローマ字列からひらがな文字列に変換する為のテーブルを管理するクラスです。</summary>
        ///<param name="aCSV">
        ///<para>ローマ字列からひらがな文字列への変換テーブルを定義したファイルアセット</para>
        ///<para>［形式］ローマ字列,ひらがな文字列,</para>
        ///<para>［例］kya,きゃ</para>
        ///</param>
        public Roma2KanaTable(in TextAsset aCSV){
            CreateTree(aCSV);
            m_romaMaxLength = m_treeRoot.GetRomaMaxLength(); 
        }
        #endregion


        #region メソッド
        /// <summary>ローマ字列[aRoma]に対して、変換できるひらがな文字列があるか</summary>
        /// <param name="aRoma">ローマ字列</param>
        /// <param name="aOutKana">(変換できる場合)変換先ひらがな文字列</param>
        /// <returns>true:打つことができる文字列がある</returns>
        public bool TryConvert(string aRoma, out string aOutKana) {
            aOutKana = "";
            Roma2KanaNode node = GetNode(aRoma);
            if (node == null) { return false; }
            if (node.HasKana) {
                aOutKana = node.Kana;
                return true; 
            }
            return false;
        }

        /// <summary>ローマ字列[aRoma]に対して、aRomaに追加でローマ字を足すことで、ひらがな文字列に変換する事が可能か</summary>
        /// <param name="aRoma">ローマ字列(例:ky)</param>
        /// <param name="aTargetKana">変換したいひらがな文字列(空文字列も可)</param>
        /// <returns>true:追加でローマ字を足すことで変換する事が可能なひらがな文字列がある</returns>
        public bool HasPossibility(string aRoma, string aTargetKana = "") {
            List<string> pKana = GetPossibilityKanas(aRoma);
            if(aTargetKana.Length == 0) {
                return pKana.Count > 0;
            }
            return pKana.BinarySearch(aTargetKana) >= 0;
        }

        /// <summary>ローマ字列[aRoma]に対して、aRomaに追加でローマ字を足すことで変換する事が可能なひらがな文字列の一覧を取得します。(ソート済み)</summary>
        /// <param name="aRoma">ローマ字列(例:ky)</param>
        /// <returns>追加でローマ字を足すことで変換する事が可能なひらがな文字列の一覧</returns>
        public List<string> GetPossibilityKanas(string aRoma)
        {
            Roma2KanaNode node = GetNode(aRoma);
            if (node == null) { return new List<string>(); }
            return node.PossibilityKanas;
        }
        #endregion


        #region 静的メソッド
        /// <summary>ローマ字列[aRoma]の先頭文字が[n]で、先頭文字を[ん]に変換できるかどうかを取得します</summary>
        /// <param name="aRoma">ローマ字列</param>
        /// <returns>true:先頭の[n]を[ん]に変換できる</returns>
        static public bool CanConverFirstN(string aRoma){
            if(aRoma.Length < 2 || aRoma[0] != 'n') { return false; };
            return Regex.IsMatch(aRoma, "^n[^aiueony]");
        }
        #endregion


        #region プロパティ
        private int m_romaMaxLength = 0;
        /// <summary>ひらがな文字列に変換できるローマ字列の最大文字数</summary>
        public int RomaMaxLength{get { return m_romaMaxLength; }}
        #endregion


        #region 内部メソッド
        ///<summary>ローマ字列からひらがな文字列に変換するためのツリーを作成</summary>
        ///<param name="aCSV">変換テーブルを定義したファイル</param>
        private void CreateTree(in TextAsset aCSV){
            const int CSV_ROMA_FIELD = 0;
            const int CSV_KANA_FIELD = 1;

            m_treeRoot = Roma2KanaNode.CreateRoot();

            CsvReadHelper csv = new CsvReadHelper(in aCSV);
            foreach (List<string> record in csv.Datas)
            {
                m_treeRoot.AddNodes(record[CSV_ROMA_FIELD], record[CSV_KANA_FIELD]);
            }

            m_treeRoot.InitAfterAddNodes();
        }

        /// <summary>ローマ字列[aRoma]に対応した木ノードオブジェクトを取得</summary>
        /// <param name="aRoma">ローマ字列(例:kya)</param>
        /// <returns>無い場合:null</returns>
        private Roma2KanaNode GetNode(string aRoma){
            Roma2KanaNode ret = m_treeRoot;
            for(int i = 0;i < aRoma.Length; ++i){
                if (ret.HasKana) { return null; }
                if (ret.Children == null) { return null; }
                if (!Util.IsAlpha(aRoma[i])) { return null; }
                ret = ret.Children[Roma2KanaNode.Alpha2ChildIdx(char.ToLower(aRoma[i]))];
                if (ret == null) { return null; }
            }
            return ret;
        }
        #endregion


        #region メンバ
        private Roma2KanaNode m_treeRoot;
        #endregion
    }


    /// <summary>ローマ字列からひらがな文字列に変換する為の、木ノードオブジェクト</summary>
    /// <example><code>
    /// using Inner;
    /// 
    ///     ...
    ///     
    /// //初期化処理
    /// Roma2KanaNode root = Roma2KanaNode.CreateRoot();
    ///
    /// //木ノードの追加
    /// root.AddNodes("a", "あ");
    /// root.AddNodes("si", "し");
    /// root.AddNodes("sa", "さ");
    /// root.AddNodes("shi", "し");
    /// root.AddNodes("gya", "ぎゃ");
    /// 
    /// 木ノード追加終了後、呼び出してください。
    /// root.InitAfterAddNodes();
    /// 
    /// </code></example>
    public class Roma2KanaNode{


        #region 生成
        ///<summary>
        /// <para>ルートノード生成関数</para>
        /// <para>外からこのクラスを作成する場合は、この関数を使用してください。</para>
        /// </summary>
        static public Roma2KanaNode CreateRoot(){
            return new Roma2KanaNode("", "");
        }
        #endregion


        #region メソッド
        /// <summary>ローマ字列からひらがな文字列に変換する為の、新たな木ノードを追加(場合によっては、内部で再帰的に呼び出します)</summary>
        /// <param name="aRoma">ローマ字列</param>
        /// <param name="aKana">変換先のひらがな</param>
        public void AddNodes(string aRoma, string aKana){
#if UNITY_EDITOR
            Debug.Assert(aRoma.Length > Depth, "Roma2KanaTable::AddNodes() パラメータの不備");
            Debug.Assert(aKana.Length > 0, "Roma2KanaTable::AddNodes() パラメータの不備");
            if (Depth > 0){
                Debug.Assert(string.Compare(aRoma, 0, Roma, 0, Depth - 1) == 0, "Roma2KanaTable::AddNodes() パラメータの不備");
            }
#endif
            char chAlpha = aRoma[Depth];
            int chIdx = Alpha2ChildIdx(chAlpha);

            if(m_children == null){
                m_children = new List<Roma2KanaNode>();
                int childNum = AlphaNum();
                for (int i = 0; i < childNum; ++i){
                    m_children.Add(null);
                }
            }

            if (aRoma.Length == Depth + 1){ //ひらがな変換可能ノード
                if (m_children[chIdx] == null) {
                    m_children[chIdx] = new Roma2KanaNode(aRoma.Substring(0, Depth + 1), aKana);
                }
                else{
                    m_children[chIdx]._SetKana(aKana);                    
                }

            }else{//途中のノード
                if (m_children[chIdx] == null)
                {
                    m_children[chIdx] = new Roma2KanaNode(aRoma.Substring(0, Depth + 1), "");
                }
                m_children[chIdx].AddNodes(aRoma, aKana);
            }

            m_possibilityKanas.Add(string.Copy(aKana));
        }

        /// <summary>
        /// <para>内部の最適化処理。重複削除やソートの処理を行います。</para>
        /// <para>ノード追加終了後、必ず呼び出してください。</para>
        /// </summary>
        public void InitAfterAddNodes(){
            m_possibilityKanas = m_possibilityKanas.Distinct().ToList();
            m_possibilityKanas.Sort();

            if(Children != null){
                foreach(Roma2KanaNode child in Children){
                    if (child != null){
                        child.InitAfterAddNodes();
                    }
                }
            }
        }

        /// <summary>全ノーツ内で、ローマ字文字列の最長の長さを取得</summary>
        /// <returns>ローマ字列の最長の長さ</returns>
        public int GetRomaMaxLength(){
            int ret = Depth;
            if (Children != null){
                foreach (Roma2KanaNode child in Children){
                    if (child != null){
                        ret = Mathf.Max(ret ,child.GetRomaMaxLength());
                    }
                }
            }
            return ret;
        }
        #endregion


        #region プロパティ
        /// <summary>
        /// <para>このノードの末尾アルファベット</para>
        /// <para>ルートノードの場合は[\0]を返却</para>
        /// </summary>
        public char Alpha{
            get{
                if (Depth == 0) { return '\0'; }
                return Roma[Roma.Length - 1];
            }
        }

        /// <summary>このノードまでのローマ字列</summary>
        public string Roma { get; private set; }

        /// <summary>このノードがひらがな文字列に変換可能かどうか</summary>
        public bool HasKana { get; private set; }

        /// <summary>
        /// <para>変換先のひらがな</para>
        /// <para>ない場合は空文字列が返却されます</para>
        /// </summary>
        public string Kana { get; private set; }

        /// <summary>このノードの深さ</summary>
        public int Depth{get { return Roma.Length; }}

        private List<Roma2KanaNode> m_children;
        /// <summary>子ノード</summary>
        public List<Roma2KanaNode> Children{get { return m_children; }}

        private List<string> m_possibilityKanas = new List<string>();
        /// <summary>このノードのローマ字列[Roma]に対して、Romaに追加でローマ字を足すことで変換する事が可能なひらがな文字列の一覧(ソート済み)</summary>
        public List<string> PossibilityKanas{get { return m_possibilityKanas; }}
        #endregion


        #region 静的メソッド
        /// <summary>アルファベットから、子ノードアクセス用の配列idxを取得します</summary>
        /// <param name="aAlpha">/a-zA-Z/</param>
        /// <returns>aなら0、Dなら4などを返却</returns>

        static public int Alpha2ChildIdx(char aAlpha){return (int)char.ToLower(aAlpha) - (int)'a';}

        /// <summary>アルファベットの総数を取得</summary>
        /// <returns>a～zの数</returns>
        static public int AlphaNum(){return (int)'z' - (int)'a' + 1;}
        #endregion


        #region 内部メソッド
        /// <summary>
        /// <para>クラス内部からの生成用。</para>
        /// <para>ローマ字列からひらがな文字列に変換する為の、木ノードオブジェクト</para>
        /// </summary>
        /// <param name="aRoma">このノードまでのローマ字列。全て英小文字で入力してください。</param>
        /// <param name="aKana">変換先のひらがな、ない場合は空文字</param>
        private Roma2KanaNode(string aRoma, string aKana = ""){
#if UNITY_EDITOR
            for (int i = 0; i < aRoma.Length; ++i){
                Debug.Assert(char.IsLower(aRoma[i]), "Roma2KanaTable::Roma2KanaNode() aRoma内に英語以外の文字を検知");
            }
#endif
            Roma = aRoma;
            HasKana = (aKana.Length != 0);
            Kana = aKana;
        }

        /// <summary>
        /// <para>外部からは使用しないでください。</para>
        /// <para>後からこのノードの変換先ひらがなをセット</para>
        /// </summary>
        /// <param name="aKana">変換先のひらがな</param>
        public void _SetKana(string aKana){
            HasKana = (aKana.Length != 0);
            Kana = aKana;
        }
        #endregion
    }
}
}