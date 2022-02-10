using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using tpInner;

///<summary>
///�L�[�{�[�h�̓��͂����ɁA�������͂̔����S�����郂�W���[���ł��B
///CPU�������ׂ�A�������v�̓I�Ȗ��ŁA���ɗ��R���Ȃ��ꍇ�́A�}�l�W�����g�N���X�ŊǗ���������ǂ������ł��B
/// </summary>
public class TypeModule : MonoBehaviour
{
#region ���͔��胂�[�h
    ///<summary>�������͔��胂�[�h�ł��B</summary>
    public enum MODE
    {
        ///<summary>
        ///�L�[�{�[�h�̓��͂����ɁA������̐������G�~�����[�g���܂��B
        ///�L�[�{�[�h���͂��當������擾�������ꍇ�Ɏg�p���Ă��������B
        ///</summary>
        MODE_INPUT,
        ///<summary>
        ///�L�[�{�[�h�̓��͂����ɁA������̐������G�~�����[�g������
        ///�ym_targetText�z�Ŏw�肳�ꂽ������Ɣ�r���A���̌��ʂ��i�[���܂��B
        ///�^�C�s���O�Q�[���ŁA����̕����ƈ�v���邩�ǂ������m�F�������ꍇ�ɗ��p���Ă��������B
        ///</summary>
        MODE_COMPARE,
    }
#endregion

#region Unity���ʏ���
    void Start()
    {
        if (KeyCode2CharCsv == null)
        {
            TextAsset tmp = new TextAsset();
            tmp = Resources.Load("Scripts/TypeModule/data/KeyCode2Char/qwerty", typeof(TextAsset)) as TextAsset;
            Debug.Assert(tmp != null, "TypeModule::�f�t�H���g�w���KeyCode2CharCsv�̓ǂݍ��ݎ��s�BTypeModule�̃t�@�C���p�X���m�F���Ă��������B");
            KeyCode2CharCsv = tmp;
        }
        LoadKeyCode2CharTable();
        if (m_roma2KanaCsv == null)
        {
            TextAsset tmp = new TextAsset();
            tmp = Resources.Load("Scripts/TypeModule/data/Char2Kana/roma", typeof(TextAsset)) as TextAsset;
            Debug.Assert(tmp != null, "TypeModule::�f�t�H���g�w���Roma2KanaCsv�̓ǂݍ��ݎ��s�BTypeModule�̃t�@�C���p�X���m�F���Ă��������B");
            Roma2KanaCsv = tmp;
        }
        LoadRomaToKanaTable();
        if (m_keyCode2KanaMidCsv == null)
        {
            TextAsset tmp = new TextAsset();
            tmp = Resources.Load("Scripts/TypeModule/data/KeyCode2Char/JISkana", typeof(TextAsset)) as TextAsset;
            Debug.Assert(tmp != null, "TypeModule::�f�t�H���g�w���KeyCode2KanaMidCsv�̓ǂݍ��ݎ��s�BTypeModule�̃t�@�C���p�X���m�F���Ă��������B");
            KeyCode2KanaMidCsv = tmp;
        }
        LoadKeyCodeToKanaMidTable();
    }

    void Update()
    {
    }

    private void OnGUI()
    {
        if (Event.current.type == EventType.KeyDown && Event.current.keyCode != KeyCode.None)
        {
            if (IsKana)
            {
                Debug.Log(m_key2kanaMid.Convert(Event.current.keyCode, Event.current.shift, Event.current.functionKey));
            }
            else
            {
                Debug.Log(m_key2char.Convert(Event.current.keyCode, Event.current.shift, Event.current.functionKey));
            }
        }
    }
#endregion

#region ���\�b�h
    //public void Clear(){}
#endregion

#region �v���p�e�B
    [Tooltip(
        "�������͔��胂�[�h�ł��B\n" +
        "�yMODE_INPUT�z   :�L�[�{�[�h�̓��͂����ɁA������̐������G�~�����[�g���܂��B\n" +
        "                   �L�[�{�[�h���͂��當������擾�������ꍇ�Ɏg�p���Ă��������B\n" +
        "�yMODE_COMPARE�z :�L�[�{�[�h�̓��͂����ɁA������̐������G�~�����[�g������\n" +
        "                  �ym_targetText�z�Ŏw�肳�ꂽ������Ɣ�r���A���̌��ʂ��i�[���܂��B\n" +
        "                  �^�C�s���O�Q�[���ŁA����̕����ƈ�v���邩�ǂ������m�F�������ꍇ�ɗ��p���Ă��������B"
            )]
    [SerializeField] private MODE m_mode = MODE.MODE_INPUT;
    public MODE Mode
    {
        get { return m_mode; }
        set { m_mode = value; }
    }

    [Header("�ȉ��ڍאݒ� (�w�肵�Ȃ��Ă������܂�)")]
    [Tooltip(
        "�L�[�̓���(KeyCode)����P�̕����ւ̕ϊ��e�[�u�����`�����t�@�C��\n" +
        "�����I�Ɏw�肵�Ȃ������ꍇ�A�ȉ��̃t�@�C����ǂݍ��݂܂��B\n" +
        "�yAssets/Resources/Scripts/TypeModule/data/KeyCode2Char/qwerty.csv�z\n" +
        "�Ǝ��Ŏw�肷��ꍇ�́A�ȉ��̂悤��CSV(.csv)�`���t�@�C����p�ӂ��Ă��������B�����R�[�h��[UTF-8]�Ƃ��Ă��������B\n" +
        "�ϊ��敶��,�yUnityEngine.KeyCode�z, isShift, isFunction\n" +
        "��) \n" +
        "S,115,1,0\n" +
        "s,115,0,0\n"

       )]
    [SerializeField] private TextAsset m_keyCode2CharCsv;
    public TextAsset KeyCode2CharCsv
    {
        get { return m_keyCode2CharCsv; }
        set
        {
            if (m_keyCode2CharCsv != value)
            {
                m_keyCode2CharCsv = value;
                LoadKeyCode2CharTable();
            }
        }
    }

    [Tooltip(
        "���[�}�������񂩂�Ђ炪�ȕ�����ւ̕ϊ��e�[�u�����`�����t�@�C��\n" +
        "�����I�Ɏw�肵�Ȃ������ꍇ�A�ȉ��̃t�@�C����ǂݍ��݂܂��B\n" +
        "�yAssets/Resources/Scripts/TypeModule/data/Char2Kana/roma.csv�z\n" +
        "�Ǝ��Ŏw�肷��ꍇ�́A�ȉ��̂悤��CSV(.csv)�`���t�@�C����p�ӂ��Ă��������B�����R�[�h��[UTF-8]�Ƃ��Ă��������B\n" +
        "���[�}��������,�Ђ炪�ȕ�����\n" +
        "��) \n" +
        "a,��\n" +
        "shi,��\n"
   )]
    [SerializeField] private TextAsset m_roma2KanaCsv;
    public TextAsset Roma2KanaCsv
    {
        get { return m_roma2KanaCsv; }
        set
        {
            if (m_roma2KanaCsv != value)
            {
                m_roma2KanaCsv = value;
                LoadKeyCode2CharTable();
            }
        }
    }

    [Space(10)]
    [Tooltip(
        "�L�[�̓���(KeyCode)���當���̕ϊ��e�[�u�����`�����t�@�C��\n" +
        "JIS���ȓ��͂ȂǁA���{��𒼐ړ��͂���������g�p����ۂɎQ�Ƃ��܂��B\n" +
        "�����I�Ɏw�肵�Ȃ������ꍇ�A�ȉ��̃t�@�C����ǂݍ��݂܂��B\n" +
        "�yAssets/Resources/Scripts/TypeModule/data/KeyCode2Char/JISkana.csv�z\n" +
        "�Ǝ��Ŏw�肷��ꍇ�́A�ȉ��̂悤��CSV(.csv)�`���t�@�C����p�ӂ��Ă��������B�����R�[�h��[UTF-8]�Ƃ��Ă��������B\n" +
        "�ϊ��敶��,�yUnityEngine.KeyCode�z, isShift, isFunction\n" +
        "��) \n" +
        "��,49,0,0 \n" +
        "��,49,1,0 \n" +
        "��,50,0,0 \n"
       )]
    [SerializeField] private TextAsset m_keyCode2KanaMidCsv;
    public TextAsset KeyCode2KanaMidCsv
    {
        get { return m_keyCode2KanaMidCsv; }
        set
        {
            if (m_keyCode2KanaMidCsv != value)
            {
                m_keyCode2KanaMidCsv = value;
                LoadKeyCodeToKanaMidTable();
            }
        }
    }

    [Space(10)]
    [Tooltip(
        "JIS���ȓ��͂ȂǁA���{��𒼐ړ��͂���������g�p���ăG�~�����[�g���邩�ǂ����̃t���O�ł��B\n" +
        "[MODE_INPUT]true�̏ꍇ�A[m_keyCodeToKanaCsv]���當���񐶐����G�~�����[�g���܂��B\n" +
        "[MODE_COMPARE]true�̏ꍇ�A���{��Ɣ�r����ꍇ��[m_keyCodeToKanaCsv]����A�����łȂ��ꍇ��[m_keyCode2CharCsv]���當���񐶐����G�~�����[�g���܂��B\n"
        )]
    [SerializeField] private bool m_isKana = false;
    public bool IsKana
    {
        get { return m_isKana; }
        set { m_isKana = value; }
    }

    [Tooltip(
       "CapsLock�̏�Ԃ𔽉f�����邩�ǂ����B\n" +
       "[true]�̏ꍇ�ACapsLock���́A�p��̓��͂ɑ΂��đ召�����𔽓]�����܂��B"
       )]
    [SerializeField] private bool m_isCheckCapsLock = true;
    public bool IsCheckCapsLock
    {
        get { return m_isCheckCapsLock; }
        set
        {
            m_isCheckCapsLock = value;
            m_key2char.IsCheckCapsLock = value;
        }
    }
#endregion

#region �������\�b�h
    ///<summary>
    ///�L�[�̓���(KeyCode) => �p�ꕶ���ւ̕ϊ��e�[�u�����쐬
    ///</summary>
    private void LoadKeyCode2CharTable()
    {
        m_key2char = new keyCode2CharTable(KeyCode2CharCsv);
        m_key2char.IsCheckCapsLock = IsCheckCapsLock;
    }

    ///<summary>
    ///���[�}�������� => �Ђ炪�ȕ�����ւ̕ϊ��e�[�u�����쐬
    ///</summary>
    private void LoadRomaToKanaTable()
    {
        m_roma2Kana = new Roma2KanaTable(Roma2KanaCsv);
    }

    ///<summary>
    ///�L�[�̓���(KeyCode) => �����ւ̕ϊ��e�[�u�����쐬
    ///</summary>
    private void LoadKeyCodeToKanaMidTable()
    {
        m_key2kanaMid = new keyCode2CharTable(KeyCode2KanaMidCsv);
        m_key2kanaMid.IsCheckCapsLock = false; //�������CapsLock�̉e�����󂯂Ȃ�
    }
#endregion

#region �����o
    keyCode2CharTable       m_key2char;
    keyCode2CharTable       m_key2kanaMid;
    Roma2KanaTable          m_roma2Kana;
#endregion 
}
