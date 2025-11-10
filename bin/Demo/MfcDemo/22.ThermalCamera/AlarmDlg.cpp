// AlarmDlg.cpp : implementation file
//

#include "stdafx.h"
#include "ThermalCamera.h"
#include "AlarmDlg.h"
#include "ThermalCameraDlg.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CAlarmDlg dialog


CAlarmDlg::CAlarmDlg(CWnd* pParent /*=NULL*/,LLONG iLoginId)
	: CDialog(CAlarmDlg::IDD, pParent)
{
	//{{AFX_DATA_INIT(CAlarmDlg)
		// NOTE: the ClassWizard will add member initialization here
	//}}AFX_DATA_INIT
    m_iLoginID = iLoginId;
	m_nAlarmIndex = -1;
}


void CAlarmDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CAlarmDlg)
	DDX_Control(pDX, IDC_LIST_ALARM, m_List);
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(CAlarmDlg, CDialog)
	//{{AFX_MSG_MAP(CAlarmDlg)
	ON_BN_CLICKED(IDC_STARTLISTEN, OnStartlisten)
    ON_MESSAGE(WM_ALARM_INFO, OnAlarmInfo)
	ON_BN_CLICKED(IDC_STOPLISTEN, OnStoplisten)
	ON_WM_DESTROY()
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CAlarmDlg message handlers

BOOL CAlarmDlg::OnInitDialog() 
{
	CDialog::OnInitDialog();
	g_SetWndStaticText(this);
	m_List.InsertColumn(0,ConvertString("Index"),LVCFMT_LEFT,60);
	m_List.InsertColumn(1,ConvertString("Alarm type"),LVCFMT_LEFT,90);
//    m_List.InsertColumn(2,ConvertString("Time"), LVCFMT_LEFT, 120);   //删去了时间的显示，因为协议内没有报警时间
	m_List.InsertColumn(2,ConvertString("Channel") , LVCFMT_LEFT, 70);
	//m_List.InsertColumn(3,ConvertString("Coordinate"), LVCFMT_LEFT,90); // 坐标点没什么用
	m_List.InsertColumn(4,ConvertString("Info"), LVCFMT_LEFT, 500);     //将原来的400改成了500，就能在demo里面完整地显示信息了

	UpdateData(FALSE);
	
	return TRUE;  // return TRUE unless you set the focus to a control
	              // EXCEPTION: OCX Property Pages should return FALSE
}

// 每个报警事件的信息，作为投递消息的载体，用户管理内存
typedef struct tagAlarmInfo
{
    long	lCommand;
    char*	pBuf;
    DWORD	dwBufLen;
    
    tagAlarmInfo()
    {
        lCommand = 0;
        pBuf = NULL;
        dwBufLen = 0;
    }
    
    ~tagAlarmInfo()
    {
        if (pBuf)
        {
            delete []pBuf;
        }
    }
}AlarmInfo;

BOOL CALLBACK MessCallBack(LONG lCommand, LLONG lLoginID, char *pBuf,
                           DWORD dwBufLen, char *pchDVRIP, LONG nDVRPort, LDWORD dwUser)
{
    if(!dwUser) 
    {
        return FALSE;
    }
    
    CAlarmDlg* dlg = (CAlarmDlg*)dwUser;
    BOOL bRet = FALSE;
    if (dlg != NULL && dlg->GetSafeHwnd())
    {
        AlarmInfo* pInfo = new AlarmInfo;
        if (!pInfo)
        {
            return FALSE;
        }
        pInfo->lCommand = lCommand;
        pInfo->pBuf = new char[dwBufLen];
        if (!pInfo->pBuf)
        {
            delete pInfo;
            return FALSE;
        }
        memcpy(pInfo->pBuf, pBuf, dwBufLen);
        pInfo->dwBufLen = dwBufLen;
        {
            TRACE("MessCallBack triggered %08x in AlarmDevice...\n", pInfo->lCommand);
        }
        dlg->PostMessage(WM_ALARM_INFO, (WPARAM)pInfo, (LPARAM)0);
    }
    
    return bRet;	
}
void CAlarmDlg::OnStartlisten() 
{
    if (!m_iLoginID)
    {
        MessageBox(ConvertString("Please login device first"), ConvertString("Prompt"));
        return;
    }
    
    // 设置回调接口
    CLIENT_SetDVRMessCallBack(MessCallBack, (LDWORD)this);
    
    BOOL bRet = CLIENT_StartListenEx(m_iLoginID);
    if (!bRet)
    {
        MessageBox(ConvertString("Subscribe failed"), ConvertString("Prompt"));
        return;
    }
    
    m_nAlarmIndex = 0;
    m_List.DeleteAllItems();
    
    //MessageBox("Subscribe ok.", ConvertString("Prompt"));
    
    GetDlgItem(IDC_STARTLISTEN)->EnableWindow(FALSE);
    GetDlgItem(IDC_STOPLISTEN)->EnableWindow(TRUE);
    
	return;
}


LRESULT CAlarmDlg::OnAlarmInfo(WPARAM wParam, LPARAM lParam)
{
    AlarmInfo* pAlarmInfo = (AlarmInfo*)wParam;
    if (!pAlarmInfo || !pAlarmInfo->pBuf || pAlarmInfo->dwBufLen <= 0)
    {
        return -1;
    }
    
    SYSTEMTIME st;
    GetLocalTime(&st);
    TRACE("%04d-%02d-%02d %02d:%02d:%02d.%03d Alarm callback: cmd:%08x, buflen:%08x,\n", 
        st.wYear, st.wMonth, st.wDay, st.wHour, st.wMinute, st.wSecond,
		pAlarmInfo->lCommand, pAlarmInfo->dwBufLen);

    if (pAlarmInfo->lCommand == DH_ALARM_HEATIMG_TEMPER)
    {
        ALARM_HEATIMG_TEMPER_INFO *AlarmHeatimgInfo = (ALARM_HEATIMG_TEMPER_INFO*)pAlarmInfo->pBuf;
        if (m_nAlarmIndex >= MAX_MSG_SHOW)
        {
            m_nAlarmIndex = 0;
            m_List.DeleteAllItems();
	    }
        char szIndex[32] = {0};
        _itoa(m_nAlarmIndex + 1, szIndex, 10);
        m_List.InsertItem(m_nAlarmIndex, NULL);
        
	    m_List.SetItemText(m_nAlarmIndex, 0, szIndex);
        m_List.SetItemText(m_nAlarmIndex, 1, ConvertString("Thermal temperature abnormal event alarm"));
        CString str;
        str.Format("%04d-%02d-%02d %02d:%02d:%02d",st.wYear, st.wMonth, st.wDay, st.wHour, st.wMinute, st.wSecond);
//        m_List.SetItemText(m_nAlarmIndex, 2, str);   //删去时间的显示，因为协议内没有定义时间。
        str.Format("channel:%d",AlarmHeatimgInfo->nChannel);
        m_List.SetItemText(m_nAlarmIndex, 2, str);
        str.Format("x:%d y:%d",AlarmHeatimgInfo->stCoordinate.nx,AlarmHeatimgInfo->stCoordinate.ny);
        //m_List.SetItemText(m_nAlarmIndex,3,str);
        CString str1;
        str1 = AlarmHeatimgInfo->szName;
        str = ConvertString("name:") + str1+" ";
        str1.Format("AlarmId:%d",AlarmHeatimgInfo->nAlarmId);
        str = str + str1+" ";
        str1 =ConvertString("Result:");
        if (AlarmHeatimgInfo->nAlarmId == NET_RADIOMETRY_RESULT_UNKNOWN)
        {
            str1 += ConvertString("unknown");
        }
        else if (AlarmHeatimgInfo->nAlarmId == NET_RADIOMETRY_RESULT_VAL)
        {
            str1+= ConvertString("concrete value");
        }
        else if (AlarmHeatimgInfo->nAlarmId == NET_RADIOMETRY_RESULT_MAX)
        {
            str1 += ConvertString("max");
        }
        else if (AlarmHeatimgInfo->nAlarmId == NET_RADIOMETRY_RESULT_MIN)
        {
            str1 += ConvertString("min");
        }
        else if (AlarmHeatimgInfo->nAlarmId == NET_RADIOMETRY_RESULT_AVR)
        {
            str1 +=ConvertString("average");
        }
        else if (AlarmHeatimgInfo->nAlarmId == NET_RADIOMETRY_RESULT_STD)
        {
            str1 +=ConvertString("standard");
        }
        else if (AlarmHeatimgInfo->nAlarmId == NET_RADIOMETRY_RESULT_MID)
        {
            str1 +=ConvertString("middle");
        }
        else if (AlarmHeatimgInfo->nAlarmId == NET_RADIOMETRY_RESULT_ISO)
        {
            str1 +=ConvertString("ISO");
        }
        str +=str1+" " ;
        str1 = ConvertString("condition:");
        if (AlarmHeatimgInfo->nAlarmContion == NET_RADIOMETRY_ALARMCONTION_UNKNOWN)
        {
            str1 +=ConvertString("unknown");
        }
        else if (AlarmHeatimgInfo->nAlarmContion == NET_RADIOMETRY_ALARMCONTION_BELOW)
        {
            str1 += ConvertString("under");
        }
        else if (AlarmHeatimgInfo->nAlarmContion == NET_RADIOMETRY_ALARMCONTION_MATCH)
        {
            str1 += ConvertString("equal");
        }
        else if (AlarmHeatimgInfo->nAlarmContion == NET_RADIOMETRY_ALARMCONTION_ABOVE)
        {
            str1 += ConvertString("above");
        }
        str += str1+" ";
        str1.Format("value:%f",AlarmHeatimgInfo->fTemperatureValue);   
        str += str1+" ";
        str1 = ConvertString("unit:");
        if (AlarmHeatimgInfo->nTemperatureUnit == TEMPERATURE_UNIT_UNKNOWN)
        {
             str1+=ConvertString("unknown");
        }
        else if (AlarmHeatimgInfo->nTemperatureUnit == TEMPERATURE_UNIT_CENTIGRADE)
        {
            str1 +=ConvertString("Celsius");
        }
        else if (AlarmHeatimgInfo->nTemperatureUnit == TEMPERATURE_UNIT_FAHRENHEIT)
        {
            str1 +=ConvertString("Fahrenheit");
        }
        str += str1 + " ";
        str1.Format("Presentid:%d",AlarmHeatimgInfo->nPresetID);   
        str +=str1;
        m_List.SetItemText(m_nAlarmIndex,3,str);
        m_nAlarmIndex++;
    }
    UpdateData(FALSE);
    return 0;
}

void CAlarmDlg::OnStoplisten() 
{
    if(m_iLoginID == 0)
    {
        MessageBox(ConvertString("Please login device first"), ConvertString("Prompt"));
        return;
    }
    
    CLIENT_StopListen(m_iLoginID);
    
    MessageBox(ConvertString("Stop Subscribe ok"), ConvertString("Prompt"));
    
    GetDlgItem(IDC_STARTLISTEN)->EnableWindow(TRUE);
	GetDlgItem(IDC_STOPLISTEN)->EnableWindow(FALSE);
}

void CAlarmDlg::OnDestroy() 
{
	CDialog::OnDestroy();
	
	CLIENT_StopListen(m_iLoginID);
}
