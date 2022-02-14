using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace tpInner{
    ///<summary>UnityのCapsLockは、押してある状態か否かで返すので使えない</summary>
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
    using System.Runtime.InteropServices;
    public static class WindowsUtil{
        [DllImport("user32.dll")]
        public static extern short GetKeyState(int keyCode);

        ///<summary>CapsLock の状態を取得</summary>
        ///<returns>[true]:CapsLockはOn</returns>
        public static bool IsCapsLockOn
            => (((ushort)GetKeyState(0x14)) & 0xffff) != 0;
    }
#else
    public static class WindowsUtil {
        public static bool IsCapsLockOn
            => return false;
    }
#endif


    ///<summary>キーの入力(KeyCode)から、単体文字へ変換する為のテーブルを管理するクラスです。</summary>
    /// <example><code>
    /// using tpInner;
    /// 
    ///     ...
    ///     
    /// //初期化処理
    /// keyCode2CharTable table = new keyCode2CharTable(in csvSrc);
    ///     
    /// //CapsLockの状態を無視
    /// table.IsCheckCapsLock = false;
    ///     
    ///     ...
    ///     
    /// //キーコードから文字に変換し、ログに出力
    /// private void OnGUI()
    /// {
    ///    if (Event.current.type == EventType.KeyDown &amp;&amp; Event.current.keyCode != KeyCode.None)
    ///    {
    ///        Debug.Log(table.Convert(Event.current.keyCode, Event.current.shift, Event.current.functionKey));
    ///    }
    /// }
    /// 
    /// </code></example>
    public class keyCode2CharTable{

        #region 生成
        ///<summary>キーの入力(KeyCode)から、単体文字へ変換する為のテーブルを管理するクラスです。</summary>
        ///<param name="aCSV">
        ///<para>キーの入力(KeyCode)から単体文字への変換テーブルを定義したファイルアセット</para>
        ///<para>［形式］変換先文字,【UnityEngine.KeyCode】, isShift, isFn</para>
        ///<para>［例］S,115,1,0</para>
        ///</param>
        public keyCode2CharTable(in TextAsset aCSV){
            CreateTable(aCSV);
            IsCheckCapsLock = true;
        }
        #endregion

        #region メソッド
        ///<summary>キーの入力(KeyCode)から単体文字へ変換</summary>
        ///<param name="aKeyCode">キーの入力値</param>
        ///<param name="aIsShift">Shiftキーが押された状態か</param>
        ///<param name="aIsFn">Fnキーか(2箇所ある\の判別等に使用する為必須)</param>
        ///<returns>単体文字</returns>
        public char Convert(UnityEngine.KeyCode aKeyCode, bool aIsShift, bool aIsFn){
            char ret;
            int key = (int)aKeyCode;
            if (aIsShift) {key += SHIFT_OFS; }
            if (aIsFn) { key += FN_OFS; }

            if (!m_map.TryGetValue(key, out ret)){ret = '\0';}

            //CapsLock中なら、アルファベットの大文字小文字を反転
            if (IsCheckCapsLock && WindowsUtil.IsCapsLockOn){
                ret = char.IsLower(ret) ? char.ToUpper(ret) : char.IsUpper(ret) ? char.ToLower(ret) : ret;
            }
            return ret;
        }
        #endregion

        #region プロパティ
        ///<summary>
        ///<para>CapsLockの状態を反映させるかどうか。</para>
        ///<para>[true]の場合、CapsLock中には、英語の大小文字を反転させます。</para>
        /// </summary>
        public bool IsCheckCapsLock { get; set; }
        #endregion

        #region 内部メソッド
        ///<summary>キーの入力(KeyCode)から単体文字への変換テーブルを作成</summary>
        ///<param name="aCSV">変換テーブルを定義したファイル</param>
        private void CreateTable(in TextAsset aCSV){
            const int CSV_CHAR_FIELD = 0;
            const int CSV_KEYCODE_FIELD = 1;
            const int CSV_SHIFT_FIELD = 2;
            const int CSV_FN_FIELD = 3;

            CsvReadHelper csv = new CsvReadHelper(in aCSV);
            foreach (List<string> record in csv.Datas){
                int key = int.Parse(record[CSV_KEYCODE_FIELD]);
                if (int.Parse(record[CSV_SHIFT_FIELD]) == 1){
                    key += SHIFT_OFS;
                }
                if (int.Parse(record[CSV_FN_FIELD]) == 1){
                    key += FN_OFS;
                }
                m_map.Add(key, record[CSV_CHAR_FIELD][0]);
            }
        }
        #endregion

        #region メンバ
        private SortedDictionary<int, char> m_map       = new SortedDictionary<int, char>();
        #endregion

        #region 内部定数
        private const int SHIFT_OFS     = (1 << 9);
        private const int FN_OFS        = (1 << 10);
        #endregion 
    }
}