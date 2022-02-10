using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace tpInner
{
    ///<summary>
    ///Unity��CapsLock�́A�����Ă����Ԃ��ۂ��ŕԂ��̂Ŏg���Ȃ�
    ///</summary>

#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
    using System.Runtime.InteropServices;
    public static class WindowsUtil
    {
        [DllImport("user32.dll")]
        public static extern short GetKeyState(int keyCode);

        ///<summary
        ///>CapsLock �̏�Ԃ��擾
        ///</summary>
        ///<returns>[true]:CapsLock��On</returns>
        public static bool IsCapsLockOn
            => (((ushort)GetKeyState(0x14)) & 0xffff) != 0;
    }
#else
    public static class WindowsUtil 
    {
        public static bool IsCapsLockOn
            => return false;
    }
#endif


    ///<summary>
    ///�L�[�̓���(KeyCode)����A�P�̕����֕ϊ�����ׂ̃e�[�u�����Ǘ�����N���X�ł��B
    /// </summary>
    /// <example><code>
    /// using tpInner;
    /// 
    ///     ...
    ///     
    /// //����������
    /// keyCode2CharTable table = new keyCode2CharTable(csvSrc);
    ///     
    /// //CapsLock�̏�Ԃ𖳎�
    /// table.IsCheckCapsLock = false;
    ///     
    ///     ...
    ///     
    /// //�L�[�R�[�h���當���ɕϊ����A���O�ɏo��
    /// private void OnGUI()
    /// {
    ///    if (Event.current.type == EventType.KeyDown && Event.current.keyCode != KeyCode.None)
    ///    {
    ///        Debug.Log(table.Convert(Event.current.keyCode, Event.current.shift, Event.current.functionKey));
    ///    }
    /// }
    /// 
    /// </code></example>
    public class keyCode2CharTable
    {
#region ����
        ///<summary>
        ///�L�[�̓���(KeyCode)����A�P�̕����֕ϊ�����ׂ̃e�[�u�����Ǘ�����N���X�ł��B
        ///</summary>
        ///<param name="aCSV">
        ///<para>�L�[�̓���(KeyCode)����P�̕����ւ̕ϊ��e�[�u�����`�����t�@�C��</para>
        ///<para>�m�`���n�ϊ��敶��,�yUnityEngine.KeyCode�z, isShift, isFn</para>
        ///<para>�m��nS,115,1,0</para>
        ///</param>
        public keyCode2CharTable(TextAsset aCSV)
        {
            CreateTable(aCSV);
            IsCheckCapsLock = true;
        }
#endregion

#region ���\�b�h
        ///<summary>�L�[�̓���(KeyCode)����P�̕����֕ϊ�</summary>
        ///<param name="aKeyCode">�L�[�̓��͒l</param>
        ///<param name="aIsShift">Shift�L�[�������ꂽ��Ԃ�</param>
        ///<param name="aIsFn">Fn�L�[��(2�ӏ�����\�̔��ʓ��Ɏg�p����וK�{)</param>
        ///<returns>�P�̕���</returns>
        public char Convert(UnityEngine.KeyCode aKeyCode, bool aIsShift, bool aIsFn)
        {
            char ret;
            int key = (int)aKeyCode;
            if (aIsShift) {key += SHIFT_OFS; }
            if (aIsFn) { key += FN_OFS; }

            //�݂���Ȃ��ꍇ�́A'\0'��ԋp
            if (!m_map.TryGetValue(key, out ret)){ret = '\0';}

            //CapsLock���Ȃ�A�A���t�@�x�b�g�̑啶���������𔽓]
            if (IsCheckCapsLock && WindowsUtil.IsCapsLockOn)
            {
                ret = char.IsLower(ret) ? char.ToUpper(ret) : char.IsUpper(ret) ? char.ToLower(ret) : ret;
            }
            return ret;
        }
#endregion

#region �v���p�e�B
        ///<summary>
        ///<para>CapsLock�̏�Ԃ𔽉f�����邩�ǂ����B</para>
        ///<para>[true]�̏ꍇ�ACapsLock���ɂ́A�p��̑召�����𔽓]�����܂��B</para>
        /// </summary>
        public bool IsCheckCapsLock { get; set; }
#endregion

#region �������\�b�h
        ///<summary>
        ///�L�[�̓���(KeyCode)����P�̕����ւ̕ϊ��e�[�u�����쐬
        ///</summary>
        ///<param name="aCSV">�ϊ��e�[�u�����`�����t�@�C��</param>
        private void CreateTable(TextAsset aCSV)
        {
            const int CSV_CHAR_FIELD = 0;
            const int CSV_KEYCODE_FIELD = 1;
            const int CSV_SHIFT_FIELD = 2;
            const int CSV_FN_FIELD = 3;

            csvReadHelper csv = new csvReadHelper(aCSV);
            foreach (List<string> record in csv.Datas)
            {
                int key = int.Parse(record[CSV_KEYCODE_FIELD]);
                if (int.Parse(record[CSV_SHIFT_FIELD]) == 1)
                {
                    key += SHIFT_OFS;
                }
                if (int.Parse(record[CSV_FN_FIELD]) == 1)
                {
                    key += FN_OFS;
                }
                m_map.Add(key, record[CSV_CHAR_FIELD][0]);
            }
        }
#endregion

#region �����o
        private Dictionary<int, char> m_map       = new Dictionary<int, char>();
#endregion

#region �����萔
        private const int SHIFT_OFS     = (1 << 9);
        private const int FN_OFS        = (1 << 10);
#endregion 
    }
}