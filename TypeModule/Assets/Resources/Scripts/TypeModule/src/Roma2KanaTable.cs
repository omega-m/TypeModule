using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text.RegularExpressions;

namespace tpInner
{

#region ����
    // �؍\���ɂ���̂ł͂Ȃ��ASortedDictionary�𗘗p�������������H
    //
    // PossibilityKanas�Ɋւ��ẮA���������n�_���牺�����Ɍ����āA��v���Ă��鐔�����O�Ƀ`�F�b�N���A���̂Ԃ�̃f�[�^��ԋp���邾���ŏI��肻��
    // �������A����List<string>()�Ƃ��č쐬���ԋp����Əd�����A���ƌ����Ď��O�ɐ������Ă����ƌ��ǎg�p���郁�����ʂ͓����� => list.GetRange() ���g���ƎQ�ƃR�s�[�ɂȂ�݂����@����ă������ʓI�ɂ� SortedDictionary �̂����Ȃ肢��
    //
    // Convert()   �̃A�N�Z�X���x�� n ���� log2(n)
    // �p�x����������CanConvert()�́@�ǂ����log2(n) �������A�؍\���̂��������^�[��������
    //
    // �Ƃ肠�����؍\���Ŏ������Ă����āA�������I�ȕs�����������Ȃ珑���ς��邱�Ƃɂ��܂��B
#endregion


    /// <summary>
    /// ���[�}���񂩂�Ђ炪�ȕ�����ɕϊ�����ׂ̃e�[�u�����Ǘ�����N���X�ł��B
    /// </summary>
    /// <example><code>
    /// using tpInner;
    /// 
    ///     ...
    ///     
    /// //����������
    /// Roma2KanaTable table = new Roma2KanaTable(csvSrc);
    /// 
    /// 
    /// //���[�}�������񂩂�Ђ炪�ȕ�����֕ϊ�
    /// string roma1 = "kya";
    /// string kana1 = table.Convert(roma1);
    /// if(kana1.Length &gt; 0){
    ///     //�Ђ炪�Ȃ֕ϊ��ł���
    ///     Debug.Log(kana1);    // "����"
    /// }
    /// 
    /// 
    /// //���[�}�������񂩂�w�肵���Ђ炪�ȕ�����֕ϊ��ł��邩�ǂ������擾
    /// string roma2 = "ty";
    /// string kana2 = "����";
    /// Debug.Log(table.CanConvert(roma2, kana2));           // "false"
    /// //�����łĂ�\�������邩���`�F�b�N
    /// Debug.Log(table.CanConvert(roma2, kana2, true));     // "true"
    ///
    /// 
    /// //�����łĂ�\��������Ђ炪�ȕ�����ꗗ���擾
    /// List&lt;string&gt; kanaList1 = table.PossibilityKanaList("tya");
    /// foreach(string k in kanaList1){
    ///     Debug.Log(k);                                   // none
    /// }
    /// 
    /// List&lt;string&gt; kanaList2 = table.PossibilityKanaList("ty");
    /// foreach(string k in kanaList2){
    ///     Debug.Log(k);                                   // "����","����","����" ... 
    /// }
    /// 
    /// 
    /// //[n]1���[��]�ɕϊ����鎖���ł��邩
    /// Debug.Log(Roma2KanaTable.CanConverFirstN("n"));         //false
    /// Debug.Log(Roma2KanaTable.CanConverFirstN("na"));        //false
    /// Debug.Log(Roma2KanaTable.CanConverFirstN("nt"));        //true
    /// Debug.Log(Roma2KanaTable.CanConverFirstN("any"));       //false
    /// Debug.Log(Roma2KanaTable.CanConverFirstN("nn"));        //false
    /// 
    /// </code></example>
    /// 
    public class Roma2KanaTable
    {
# region ����
        ///<summary>
        /// ���[�}���񂩂�Ђ炪�ȕ�����ɕϊ�����ׂ̃e�[�u�����Ǘ�����N���X�ł��B
        /// </summary>
        ///<param name="aCSV">
        ///<para>���[�}���񂩂�Ђ炪�ȕ�����ւ̕ϊ��e�[�u�����`�����t�@�C��</para>
        ///<para>�m�`���n���[�}����,�Ђ炪�ȕ�����,</para>
        ///<para>�m��nkya,����</para>
        ///</param>m_possibilityKanas
        public Roma2KanaTable(TextAsset aCSV)
        {
            CreateTable(aCSV);
        }
#endregion

#region ���\�b�h
        /// <summary>
        /// ���[�}����[aRoma]����ϊ��ł���Ђ炪�ȕ�������擾�B
        /// </summary>
        /// <param name="aRoma">���[�}����</param>
        /// <returns>�Ђ炪�ȕ�����A�ϊ��ł��Ȃ��ꍇ�͋󕶎���</returns>
        public string Convert(string aRoma)
        {
            Roma2KanaNode node = GetNode(aRoma);
            if (node == null) { return ""; }
            return node.Kana;
        }

        /// <summary>
        /// ���[�}����[aRoma]�ɑ΂��āA�Ђ炪��[aKana]��ł��Ƃ��ł��邩
        /// </summary>
        /// <param name="aRoma">���[�}����(��:ky)</param>
        /// <param name="aKana">�Ђ炪�ȕ�����(��:����)</param>
        /// <param name="aIsPossibility">true:���[�}����[aRoma]�ɒǉ��Ń��[�}���𑫂����Ƃőł��@�����邩���`�F�b�N</param>
        /// <returns>true:�ł��@������</returns>
        public bool CanConvert(string aRoma, string aKana, bool aIsPossibility = false)
        {
            Roma2KanaNode node = GetNode(aRoma);
            if (node == null) { return false; }
            if (string.Compare(node.Kana, aKana) == 0) { return true; }
            if (aIsPossibility)
            {
                if (node.PossibilityKanas.BinarySearch(aKana) >= 0) { return true; }
            }
            return false;
        }

        /// <summary>
        /// ���[�}����[aRoma]�ɑ΂��āAaRoma�ɒǉ��Ń��[�}���𑫂����Ƃŕϊ����鎖���\�ȂЂ炪�ȕ�����̈ꗗ���擾���܂��B(�\�[�g�ς�)
        /// </summary>
        /// <param name="aRoma">���[�}����(��:ky)</param>
        /// <returns>true:�ǉ��Ń��[�}���𑫂����Ƃŕϊ����鎖���\�ȂЂ炪�ȕ�����̈ꗗ</returns>
        public List<string> PossibilityKanaList(string aRoma)
        {
            Roma2KanaNode node = GetNode(aRoma);
            if (node == null) { return new List<string>(); }
            return node.PossibilityKanas;
        }
#endregion

#region �ÓI���\�b�h
        /// <summary>
        /// ���[�}����[aRoma]�̐擪������[n]�ŁA�擪������[��]�ɕϊ��ł��邩�ǂ������擾���܂�
        /// </summary>
        /// <param name="aRoma">���[�}����</param>
        /// <returns>true:�擪��[n]��[��]�ɕϊ��ł���</returns>
        static bool CanConverFirstN(string aRoma)
        {
            if(aRoma.Length < 2) { return false; };
            return Regex.IsMatch(aRoma, "^n[^aiueony]");
        }
#endregion

#region �������\�b�h
        ///<summary>
        ///���[�}���񂩂�Ђ炪�ȕ�����ɕϊ�����e�[�u�����쐬
        ///</summary>
        ///<param name="aCSV">�ϊ��e�[�u�����`�����t�@�C��</param>
        private void CreateTable(TextAsset aCSV)
        {
            const int CSV_ROMA_FIELD = 0;
            const int CSV_KANA_FIELD = 1;

            m_treeRoot = Roma2KanaNode.CreateRoot();

            csvReadHelper csv = new csvReadHelper(aCSV);
            foreach (List<string> record in csv.Datas)
            {
                m_treeRoot.AddNodes(record[CSV_ROMA_FIELD], record[CSV_KANA_FIELD]);
            }

            m_treeRoot.InitAfterAddNodes();
        }

        /// <summary>
        /// ���[�}����[aRoma]�ɑΉ������؃m�[�h�I�u�W�F�N�g���擾
        /// </summary>
        /// <param name="aRoma">���[�}����(��:kya)</param>
        /// <returns>�����ꍇ:null</returns>
        private Roma2KanaNode GetNode(string aRoma)
        {
#if UNITY_EDITOR
            for (int i = 0; i < aRoma.Length; ++i)
            {
                Debug.Assert(char.IsLower(char.ToLower(aRoma[i])),"Roma2KanaTable::GetNode() aRoma���ɉp��ȊO�̕��������m");
            }
#endif
            Roma2KanaNode ret = m_treeRoot;
            for(int i = 0;i < aRoma.Length; ++i)
            {
                if (ret.HasKana) { return null; }
                if (ret.Children == null) { return null; }
                ret = ret.Children[Roma2KanaNode.Alpha2NodeIdx(char.ToLower(aRoma[i]))];
                if (ret == null) { return null; }
            }
            return ret;
        }
 #endregion

#region �����o
        private Roma2KanaNode m_treeRoot;
#endregion
    }


    /// <summary>
    /// ���[�}���񂩂�Ђ炪�ȕ�����ɕϊ�����ׂ́A�؃m�[�h�I�u�W�F�N�g
    /// </summary>
    /// <example><code>
    /// using tpInner;
    /// 
    ///     ...
    ///     
    /// //����������
    /// Roma2KanaNode root = Roma2KanaNode.CreateRoot();
    ///
    /// //�؃m�[�h�̒ǉ�
    /// root.AddNodes("a", "��");
    /// root.AddNodes("si", "��");
    /// root.AddNodes("sa", "��");
    /// root.AddNodes("shi", "��");
    /// root.AddNodes("gya", "����");
    /// 
    /// �؃m�[�h�ǉ��I����A�Ăяo���Ă��������B
    /// root.InitAfterAddNodes();
    /// 
    /// </code></example>
    public class Roma2KanaNode
    {
#region ����
        ///<summary>
        /// <para>���[�g�m�[�h�����֐�</para>
        /// <para>�O���炱�̃N���X���쐬����ꍇ�́A���̊֐����g�p���Ă��������B</para>
        /// </summary>
        static public Roma2KanaNode CreateRoot()
        {
            return new Roma2KanaNode("", "");
        }
#endregion

#region ���\�b�h
        /// <summary>
        /// ���[�}���񂩂�Ђ炪�ȕ�����ɕϊ�����ׂ́A�V���Ȗ؃m�[�h��ǉ�(�ꍇ�ɂ���ẮA�����ōċA�I�ɌĂяo���܂�)
        /// </summary>
        /// <param name="aRoma">���[�}����</param>
        /// <param name="aKana">�ϊ���̂Ђ炪��</param>
        public void AddNodes(string aRoma, string aKana)
        {
#if UNITY_EDITOR
            Debug.Assert(aRoma.Length > Depth, "Roma2KanaTable::AddNodes() �p�����[�^�̕s��");
            Debug.Assert(aKana.Length > 0, "Roma2KanaTable::AddNodes() �p�����[�^�̕s��");
            if (Depth > 0)
            {
                Debug.Assert(string.Compare(aRoma, 0, Roma, 0, Depth - 1) == 0, "Roma2KanaTable::AddNodes() �p�����[�^�̕s��");
            }
#endif
            char chAlpha = aRoma[Depth];
            int chIdx = Alpha2NodeIdx(chAlpha);

            if(m_children == null)
            {
                m_children = new List<Roma2KanaNode>();
                int childNum = AlphaNum();
                for (int i = 0; i < childNum; ++i)
                {
                    m_children.Add(null);
                }
            }

            if (aRoma.Length == Depth + 1)
            { //�Ђ炪�ȕϊ��\�m�[�h
                if (m_children[chIdx] == null) {
                    m_children[chIdx] = new Roma2KanaNode(aRoma.Substring(0, Depth + 1), aKana);
                }
                else
                {
                    m_children[chIdx]._SetKana(aKana);                    
                }

            }
            else//�r���̃m�[�h
            {
                if (m_children[chIdx] == null)
                {
                    m_children[chIdx] = new Roma2KanaNode(aRoma.Substring(0, Depth + 1), "");
                }
                m_children[chIdx].AddNodes(aRoma, aKana);
            }

            m_possibilityKanas.Add(string.Copy(aKana));
        }

        /// <summary>
        /// �����̍œK�������B�d���폜��\�[�g�̏������s���܂��B
        /// �m�[�h�ǉ��I����A�K���Ăяo���Ă��������B
        /// </summary>
        public void InitAfterAddNodes()
        {
            m_possibilityKanas = m_possibilityKanas.Distinct().ToList();
            m_possibilityKanas.Sort();

            if(Children != null)
            {
                foreach(Roma2KanaNode child in Children)
                {
                    if (child != null)
                    {
                        child.InitAfterAddNodes();
                    }
                }
            }
        }

        /// <summary>
        /// �S�m�[�c���ŁA���[�}��������̍Œ��̒������擾
        /// </summary>
        /// <returns>���[�}����̍Œ��̒���</returns>
        public int GetMaxRomaLength()
        {
            int ret = Depth - 1;
            if (Children != null)
            {
                foreach (Roma2KanaNode child in Children)
                {
                    if (child != null)
                    {
                        ret = Mathf.Max(ret ,child.GetMaxRomaLength());
                    }
                }
            }
            return ret;
        }
#endregion

#region �v���p�e�B
        /// <summary>
        /// <para>���̃m�[�h�̖����A���t�@�x�b�g</para>
        /// <para>���[�g�m�[�h�̏ꍇ��[\0]��ԋp</para>
        /// </summary>
        public char Alpha
        {
            get
            {
                if (Depth == 0) { return '\0'; }
                return Roma[Roma.Length - 1];
            }
        }

        /// <summary>
        /// ���̃m�[�h�܂ł̃��[�}����
        /// </summary>
        public string Roma { get; private set; }

        /// <summary>
        /// ���̃m�[�h���Ђ炪�ȕ�����ɕϊ��\���ǂ���
        /// </summary>
        public bool HasKana { get; private set; }

        /// <summary>
        /// <para>�ϊ���̂Ђ炪��</para>
        /// <para>�Ȃ��ꍇ�͋󕶎��񂪕ԋp����܂�</para>
        /// </summary>
        public string Kana { get; private set; }

        /// <summary>
        /// ���̃m�[�h�̐[��
        /// </summary>
        public int Depth
        {
            get { return Roma.Length; }
        }

        /// <summary>
        /// �q�m�[�h
        /// </summary>
        private List<Roma2KanaNode> m_children;
        public List<Roma2KanaNode> Children
        {
            get { return m_children; }
        }

        private List<string> m_possibilityKanas = new List<string>();
        /// <summary>
        /// <para>���̃m�[�h�̃��[�}����[Roma]�ɑ΂��āARoma�ɒǉ��Ń��[�}���𑫂����Ƃŕϊ����鎖���\�ȂЂ炪�ȕ�����̈ꗗ(�\�[�g�ς�)</para>
        /// </summary>
        public List<string> PossibilityKanas
        {
            get { return m_possibilityKanas; }
        }
#endregion

#region �ÓI���\�b�h
        /// <summary>
        /// �A���t�@�x�b�g����A�q�m�[�h�A�N�Z�X�p�̔z��idx���擾���܂�
        /// </summary>
        /// <param name="aAlpha">/a-zA-Z/</param>
        /// <returns>a�Ȃ�0�AD�Ȃ�4�Ȃǂ�ԋp</returns>

        static public int Alpha2NodeIdx(char aAlpha)
        {
            return (int)char.ToLower(aAlpha) - (int)'a';
        }

        /// <summary>
        /// �A���t�@�x�b�g�̑������擾
        /// </summary>
        /// <returns>a�`z�̐�</returns>
        static public int AlphaNum()
        {
            return (int)'z' - (int)'a' + 1;
        }
#endregion

#region �������\�b�h
        /// <summary>
        /// <para>�N���X��������̐����p�B</para>
        /// <para>���[�}���񂩂�Ђ炪�ȕ�����ɕϊ�����ׂ́A�؃m�[�h�I�u�W�F�N�g</para>
        /// </summary>
        /// <param name="aRoma">���̃m�[�h�܂ł̃��[�}����B�S�ĉp�������œ��͂��Ă��������B</param>
        /// <param name="aKana">�ϊ���̂Ђ炪�ȁA�Ȃ��ꍇ�͋󕶎�</param>
        private Roma2KanaNode(string aRoma, string aKana = "")
        {
#if UNITY_EDITOR
            for (int i = 0; i < aRoma.Length; ++i)
            {
                Debug.Assert(char.IsLower(aRoma[i]), "Roma2KanaTable::Roma2KanaNode() aRoma���ɉp��ȊO�̕��������m");
            }
#endif

            Roma = aRoma;
            HasKana = (aKana.Length != 0);
            Kana = aKana;
        }

        /// <summary>
        /// <para>�O������͎g�p���Ȃ��ł��������B</para>
        /// <para>�ォ�炱�̃m�[�h�̕ϊ���Ђ炪�Ȃ��Z�b�g</para>
        /// </summary>
        /// <param name="aKana">�ϊ���̂Ђ炪��</param>
        public void _SetKana(string aKana)
        {
            HasKana = (aKana.Length != 0);
            Kana = aKana;
        }
#endregion
    }
}