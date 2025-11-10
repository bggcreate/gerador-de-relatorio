// IntelligentEventDlg.cpp : implementation file
//

#include "stdafx.h"
#include "IntelligentTraffic.h"
#include "IntelligentEventDlg.h"
          

//Traffic intelligent event Info
struct TRAFFIC_INTELLIGENT_EVENT_INFO 
{
    NET_TIME_EX    szTime;                // Event start time
    char           cEventName[32];        // Traffic event  name
    char           cPlateNumber[32];      // Plate number
    char           cPlateColor[32];       // Plate color
    char           cVehicleColor[32];     // vehicle color
    char           cVehicleSign[32];      // vehicle Sign
    int            nSpeed;                // Vehicle Speed
    int            nLane;                 // Lane   

    DWORD          dwOffSet;              // Matting picture off set
    DWORD          dwFileLength;          // End of matting picture off set
    BYTE*          pBuf;                  // Snap picture buffer
    DWORD          dwBufSize;             // Buffer size
};  


//Intelligent traffic event  call back
int CALLBACK CbIntelligentEvent(LLONG lAnalyzerHandle, DWORD dwAlarmType, void* pAlarmInfo, BYTE *pBuffer, DWORD dwBufSize, LDWORD dwUser, int nSequence, void *reserved)
{
    if (0 == dwUser)
    {
        return -1;
    }

    if (EVENT_IVS_TRAFFICJUNCTION != dwAlarmType && 
        EVENT_IVS_TRAFFIC_RETROGRADE != dwAlarmType &&
        EVENT_IVS_TRAFFICJAM != dwAlarmType &&
        EVENT_IVS_TRAFFIC_OVERSPEED != dwAlarmType &&
        EVENT_IVS_TRAFFIC_UNDERSPEED != dwAlarmType &&
        EVENT_IVS_TRAFFIC_PARKING != dwAlarmType &&
        EVENT_IVS_TRAFFIC_PEDESTRAIN != dwAlarmType )
    {
        return -1;
    }
    
    TRAFFIC_INTELLIGENT_EVENT_INFO* pTrafficEventInfo =new TRAFFIC_INTELLIGENT_EVENT_INFO;
    memset(pTrafficEventInfo, 0, sizeof(TRAFFIC_INTELLIGENT_EVENT_INFO));

    switch(dwAlarmType)
    {
    case EVENT_IVS_TRAFFICJUNCTION:   // 卡口事件
        {
            DEV_EVENT_TRAFFICJUNCTION_INFO* pEventInfo = (DEV_EVENT_TRAFFICJUNCTION_INFO*)pAlarmInfo;
            if (NULL == pEventInfo)
            {
                return -1;
            }
            pTrafficEventInfo->szTime = pEventInfo->UTC;
            pTrafficEventInfo->nLane = pEventInfo->nLane;
            strncpy_s(pTrafficEventInfo->cPlateNumber , pEventInfo->stTrafficCar.szPlateNumber, sizeof(pTrafficEventInfo->cPlateNumber)-1);
            strncpy_s(pTrafficEventInfo->cPlateColor , pEventInfo->stTrafficCar.szPlateColor, sizeof(pTrafficEventInfo->cPlateColor)-1);
            strncpy_s(pTrafficEventInfo->cEventName, "ANPR", sizeof(pTrafficEventInfo->cEventName)-1);
            strncpy_s(pTrafficEventInfo->cVehicleColor, pEventInfo->stTrafficCar.szVehicleColor, sizeof(pTrafficEventInfo->cVehicleColor)-1);
            strncpy_s(pTrafficEventInfo->cVehicleSign, pEventInfo->stTrafficCar.szVehicleSign, sizeof(pTrafficEventInfo->cVehicleSign)-1);
            pTrafficEventInfo->nSpeed = pEventInfo->nSpeed;
             char*pbuf = pEventInfo->stuVehicle.szObjectType;

            if (pEventInfo->stuObject.bPicEnble)
            {
                pTrafficEventInfo->dwOffSet = pEventInfo->stuObject.stPicInfo.dwOffSet;
                pTrafficEventInfo->dwFileLength = pEventInfo->stuObject.stPicInfo.dwFileLenth;
            }
        }
        break;

    case EVENT_IVS_TRAFFICJAM: // 交通拥堵
        {
            DEV_EVENT_TRAFFICJAM_INFO *pEventInfo = (DEV_EVENT_TRAFFICJAM_INFO*)pAlarmInfo;
            if (NULL == pEventInfo)
            {
                return -1;
            }
            pTrafficEventInfo->szTime = pEventInfo->UTC;
            pTrafficEventInfo->nLane = pEventInfo->nLane;
            strncpy_s(pTrafficEventInfo->cPlateNumber , pEventInfo->stTrafficCar.szPlateNumber, sizeof(pTrafficEventInfo->cPlateNumber)-1);
            strncpy_s(pTrafficEventInfo->cPlateColor , pEventInfo->stTrafficCar.szPlateColor, sizeof(pTrafficEventInfo->cPlateColor)-1);
            strncpy_s(pTrafficEventInfo->cEventName, "Jam", sizeof(pTrafficEventInfo->cEventName)-1);
            strncpy_s(pTrafficEventInfo->cVehicleColor, pEventInfo->stTrafficCar.szVehicleColor, sizeof(pTrafficEventInfo->cVehicleColor)-1);
            strncpy_s(pTrafficEventInfo->cVehicleSign, pEventInfo->stTrafficCar.szVehicleSign, sizeof(pTrafficEventInfo->cVehicleSign)-1);
        }
        break;

    case EVENT_IVS_TRAFFIC_OVERSPEED: // 超速
        {
            DEV_EVENT_TRAFFIC_OVERSPEED_INFO *pEventInfo = (DEV_EVENT_TRAFFIC_OVERSPEED_INFO*)pAlarmInfo;
            if (NULL == pEventInfo)
            {
                return -1;
            }
            pTrafficEventInfo->szTime = pEventInfo->UTC;
            pTrafficEventInfo->nLane = pEventInfo->nLane;
            strncpy_s(pTrafficEventInfo->cPlateNumber , pEventInfo->stTrafficCar.szPlateNumber, sizeof(pTrafficEventInfo->cPlateNumber)-1);
            strncpy_s(pTrafficEventInfo->cPlateColor , pEventInfo->stTrafficCar.szPlateColor, sizeof(pTrafficEventInfo->cPlateColor)-1);
            strncpy_s(pTrafficEventInfo->cEventName, "Over Speed", sizeof(pTrafficEventInfo->cEventName)-1);
            strncpy_s(pTrafficEventInfo->cVehicleColor, pEventInfo->stTrafficCar.szVehicleColor, sizeof(pTrafficEventInfo->cVehicleColor)-1);
            strncpy_s(pTrafficEventInfo->cVehicleSign, pEventInfo->stTrafficCar.szVehicleSign, sizeof(pTrafficEventInfo->cVehicleSign)-1);
            pTrafficEventInfo->nSpeed = pEventInfo->nSpeed;

            if (pEventInfo->stuObject.bPicEnble)
            {
                pTrafficEventInfo->dwOffSet = pEventInfo->stuObject.stPicInfo.dwOffSet;
                pTrafficEventInfo->dwFileLength = pEventInfo->stuObject.stPicInfo.dwFileLenth;
            }
        }
        break;

    case EVENT_IVS_TRAFFIC_UNDERSPEED:  // 欠速
        {
            DEV_EVENT_TRAFFIC_UNDERSPEED_INFO *pEventInfo = (DEV_EVENT_TRAFFIC_UNDERSPEED_INFO*)pAlarmInfo;
            if (NULL == pEventInfo)
            {
                return -1;
            }
            pTrafficEventInfo->szTime = pEventInfo->UTC;
            pTrafficEventInfo->nLane = pEventInfo->nLane;
            strncpy_s(pTrafficEventInfo->cPlateNumber , pEventInfo->stTrafficCar.szPlateNumber, sizeof(pTrafficEventInfo->cPlateNumber)-1);
            strncpy_s(pTrafficEventInfo->cPlateColor , pEventInfo->stTrafficCar.szPlateColor, sizeof(pTrafficEventInfo->cPlateColor)-1);
            strncpy_s(pTrafficEventInfo->cEventName, "Lack Speed", sizeof(pTrafficEventInfo->cEventName)-1);
            strncpy_s(pTrafficEventInfo->cVehicleColor, pEventInfo->stTrafficCar.szVehicleColor, sizeof(pTrafficEventInfo->cVehicleColor)-1);
            strncpy_s(pTrafficEventInfo->cVehicleSign, pEventInfo->stTrafficCar.szVehicleSign, sizeof(pTrafficEventInfo->cVehicleSign)-1);
            pTrafficEventInfo->nSpeed = pEventInfo->nSpeed;
            if (pEventInfo->stuObject.bPicEnble)
            {
                pTrafficEventInfo->dwOffSet = pEventInfo->stuObject.stPicInfo.dwOffSet;
                pTrafficEventInfo->dwFileLength = pEventInfo->stuObject.stPicInfo.dwFileLenth;
            }
        }
        break;

    case EVENT_IVS_TRAFFIC_RETROGRADE:           // 逆行
        {
            DEV_EVENT_TRAFFIC_RETROGRADE_INFO *pEventInfo = (DEV_EVENT_TRAFFIC_RETROGRADE_INFO*)pAlarmInfo;
            if (NULL == pEventInfo)
            {
                return -1;
            }
            pTrafficEventInfo->szTime = pEventInfo->UTC;
            pTrafficEventInfo->nLane = pEventInfo->nLane;
            strncpy_s(pTrafficEventInfo->cPlateNumber , pEventInfo->stTrafficCar.szPlateNumber, sizeof(pTrafficEventInfo->cPlateNumber)-1);
            strncpy_s(pTrafficEventInfo->cPlateColor , pEventInfo->stTrafficCar.szPlateColor, sizeof(pTrafficEventInfo->cPlateColor)-1);
            strncpy_s(pTrafficEventInfo->cEventName, "Retrograde", sizeof(pTrafficEventInfo->cEventName)-1);
            strncpy_s(pTrafficEventInfo->cVehicleColor, pEventInfo->stTrafficCar.szVehicleColor, sizeof(pTrafficEventInfo->cVehicleColor)-1);
            strncpy_s(pTrafficEventInfo->cVehicleSign, pEventInfo->stTrafficCar.szVehicleSign, sizeof(pTrafficEventInfo->cVehicleSign)-1);
            pTrafficEventInfo->nSpeed = pEventInfo->nSpeed;
            if (pEventInfo->stuObject.bPicEnble)
            {
                pTrafficEventInfo->dwOffSet = pEventInfo->stuObject.stPicInfo.dwOffSet;
                pTrafficEventInfo->dwFileLength = pEventInfo->stuObject.stPicInfo.dwFileLenth;
            }
        }
        break;

    case EVENT_IVS_TRAFFIC_PARKING:  // 违法停车
        {
            DEV_EVENT_TRAFFIC_PARKING_INFO *pEventInfo = (DEV_EVENT_TRAFFIC_PARKING_INFO*)pAlarmInfo;
            if (NULL == pEventInfo)
            {
                return -1;
            }
            pTrafficEventInfo->szTime = pEventInfo->UTC;
            pTrafficEventInfo->nLane = pEventInfo->nLane;
            strncpy_s(pTrafficEventInfo->cPlateNumber , pEventInfo->stTrafficCar.szPlateNumber, sizeof(pTrafficEventInfo->cPlateNumber)-1);
            strncpy_s(pTrafficEventInfo->cPlateColor , pEventInfo->stTrafficCar.szPlateColor, sizeof(pTrafficEventInfo->cPlateColor)-1);
            strncpy_s(pTrafficEventInfo->cEventName, "Illegal Parking", sizeof(pTrafficEventInfo->cEventName)-1);
            strncpy_s(pTrafficEventInfo->cVehicleColor, pEventInfo->stTrafficCar.szVehicleColor, sizeof(pTrafficEventInfo->cVehicleColor)-1);
            strncpy_s(pTrafficEventInfo->cVehicleSign, pEventInfo->stTrafficCar.szVehicleSign, sizeof(pTrafficEventInfo->cVehicleSign)-1);
            if (pEventInfo->stuObject.bPicEnble)
            {
                pTrafficEventInfo->dwOffSet = pEventInfo->stuObject.stPicInfo.dwOffSet;
                pTrafficEventInfo->dwFileLength = pEventInfo->stuObject.stPicInfo.dwFileLenth;
            }
        }
        break;
        case EVENT_IVS_TRAFFIC_PEDESTRAIN:  // 交通行人
        {
            DEV_EVENT_TRAFFIC_PEDESTRAIN_INFO *pEventInfo = (DEV_EVENT_TRAFFIC_PEDESTRAIN_INFO*)pAlarmInfo;
            if (NULL == pEventInfo)
            {
                return -1;
            }
            pTrafficEventInfo->szTime = pEventInfo->UTC;
            pTrafficEventInfo->nLane = pEventInfo->nLane;
            strncpy_s(pTrafficEventInfo->cEventName, "Pedestrain", sizeof(pTrafficEventInfo->cEventName)-1);
            if (pEventInfo->stuObject.bPicEnble)
            {
                pTrafficEventInfo->dwOffSet = pEventInfo->stuObject.stPicInfo.dwOffSet;
                pTrafficEventInfo->dwFileLength = pEventInfo->stuObject.stPicInfo.dwFileLenth;
            }
        }
        break;
    }

    pTrafficEventInfo->pBuf = new BYTE[dwBufSize];
    memcpy(pTrafficEventInfo->pBuf, pBuffer, dwBufSize);
    pTrafficEventInfo->dwBufSize = dwBufSize;
    PostMessage(((CIntelligentEventDlg *)dwUser)->GetSafeHwnd(), WM_INTELLIGENT_EVENT, (WPARAM)pTrafficEventInfo, 0);
    
    return 0;
}


// CIntelligentEventDlg Dialog

IMPLEMENT_DYNAMIC(CIntelligentEventDlg, CDialog)

CIntelligentEventDlg::CIntelligentEventDlg(CWnd* pParent /*=NULL*/)
	: CDialog(CIntelligentEventDlg::IDD, pParent)
{
    m_lLoginHandle = 0;
    m_lPlayHandle = 0;
    m_lRealLoadHandle = 0;
    m_nIndexOfEvent = 0;
}

CIntelligentEventDlg::~CIntelligentEventDlg()
{
}

void CIntelligentEventDlg::DoDataExchange(CDataExchange* pDX)
{
    CDialog::DoDataExchange(pDX);
    DDX_Control(pDX, IDC_BTN_START_PLAY_AND_STOP_PLAY, m_btnPlay);
    DDX_Control(pDX, IDC_BTN_SUBSCIRBE_AND_UNSUBSICRIBE, m_btnSubscribe);
    DDX_Control(pDX, IDC_LIST_EVENT, m_ctrEventList);
    DDX_Control(pDX, IDC_STATIC_EVENT_PICTURE, m_ctrEventPicture);
    DDX_Control(pDX, IDC_STATIC_PLATE_PICTURE, m_ctrPlatePicture);
    DDX_Control(pDX, IDC_COMBO_CHANNEL, m_cmbChannel);
}


BEGIN_MESSAGE_MAP(CIntelligentEventDlg, CDialog)
    ON_BN_CLICKED(IDC_BTN_START_PLAY_AND_STOP_PLAY, &CIntelligentEventDlg::OnBnClickedBtnStartPlayAndStopPlay)
    ON_WM_DESTROY()
    ON_BN_CLICKED(IDC_BTN_SUBSCIRBE_AND_UNSUBSICRIBE, &CIntelligentEventDlg::OnBnClickedBtnSubscirbeAndUnsubsicribe)
    ON_MESSAGE(WM_INTELLIGENT_EVENT, &CIntelligentEventDlg::OnIntelligentEvent)
END_MESSAGE_MAP()


// CIntelligentEventDlg message handlers

BOOL CIntelligentEventDlg::PreTranslateMessage(MSG* pMsg)
{
    // Enter key
    if(pMsg->message == WM_KEYDOWN &&
        pMsg->wParam == VK_RETURN)
    {
        return TRUE;
    }

    // Escape key
    if(pMsg->message == WM_KEYDOWN &&
        pMsg->wParam == VK_ESCAPE)
    {
        return TRUE;
    }
    return CDialog::PreTranslateMessage(pMsg);
}


BOOL CIntelligentEventDlg::OnInitDialog()
{
    CDialog::OnInitDialog();
    g_SetWndStaticText(this);

    m_btnPlay.EnableWindow(FALSE);
    m_btnSubscribe.EnableWindow(FALSE);
    
    m_ctrEventList.SetExtendedStyle(m_ctrEventList.GetExtendedStyle() | LVS_EX_HEADERDRAGDROP | LVS_EX_FULLROWSELECT | LVS_EX_GRIDLINES);
    m_ctrEventList.InsertColumn(0, ConvertString("Index"), LVCFMT_LEFT, 70);
    m_ctrEventList.InsertColumn(1, ConvertString("Time"), LVCFMT_LEFT, 125);
    m_ctrEventList.InsertColumn(2, ConvertString("Event Type"), LVCFMT_LEFT, 125);
    m_ctrEventList.InsertColumn(3, ConvertString("Lane"), LVCFMT_LEFT, 70);
    m_ctrEventList.InsertColumn(4, ConvertString("Plate Number"), LVCFMT_LEFT, 125);
    m_ctrEventList.InsertColumn(5, ConvertString("Plate Color"), LVCFMT_LEFT, 100);
    m_ctrEventList.InsertColumn(6, ConvertString("Vehicle Color"), LVCFMT_LEFT, 100);
    m_ctrEventList.InsertColumn(7, ConvertString("Speed(km/h)"), LVCFMT_LEFT, 100);
    m_ctrEventList.InsertColumn(8, ConvertString("Vehicle Sign"), LVCFMT_LEFT, 100);
    return TRUE;   // return TRUE unless you set the focus to a control
}

void CIntelligentEventDlg::Init(unsigned int nChannel, LLONG lLoginHandle)
{
    if (0 == nChannel || 0 == lLoginHandle)
    {
        return;
    }

    for (unsigned int i = 0 ;i < nChannel; i++)
    {
        CString csChannel;
        csChannel.Format("%d", i+1);
        m_cmbChannel.InsertString(i, csChannel);
    }
    m_cmbChannel.SetCurSel(0);

    m_lLoginHandle = lLoginHandle;
    m_btnPlay.EnableWindow(TRUE);
    m_btnSubscribe.EnableWindow(TRUE);
}

void CIntelligentEventDlg::CleanUp()
{
    if (0 != m_lRealLoadHandle)
    {
        OnBnClickedBtnSubscirbeAndUnsubsicribe();
    }
    if (0 != m_lPlayHandle)
    {
        OnBnClickedBtnStartPlayAndStopPlay();
    }

    m_cmbChannel.ResetContent();
    m_btnPlay.EnableWindow(FALSE);
    m_btnSubscribe.EnableWindow(FALSE);
}


void CIntelligentEventDlg::OnBnClickedBtnStartPlayAndStopPlay()
{
    if (0 != m_lPlayHandle)
    {
        CLIENT_StopRealPlay(m_lPlayHandle);
        m_lPlayHandle =0;
        m_btnPlay.SetWindowText(ConvertString("Start Play"));
        Invalidate(TRUE);
    }
    else
    {
        int nChannel = m_cmbChannel.GetCurSel();
        HWND hWnd = GetDlgItem(IDC_STATIC_REAL_PLAY)->GetSafeHwnd();

        m_lPlayHandle = CLIENT_StartRealPlay(m_lLoginHandle, nChannel, hWnd, DH_RType_Realplay, NULL, NULL, NULL);
        if (0 == m_lPlayHandle)
        {
            MessageBox(ConvertString("Real play failed!"), ConvertString("Prompt"));
            return;
        }
        m_btnPlay.SetWindowText(ConvertString("Stop Play"));
        CLIENT_RenderPrivateData(m_lPlayHandle, TRUE);
    }
}

void CIntelligentEventDlg::OnBnClickedBtnSubscirbeAndUnsubsicribe()
{
    if (0 != m_lRealLoadHandle)
    {
        CLIENT_StopLoadPic(m_lRealLoadHandle);
        m_lRealLoadHandle = 0;
        m_nIndexOfEvent = 0;
        m_ctrEventList.DeleteAllItems();
        m_ctrEventPicture.SetImageFile(NULL);
        m_ctrPlatePicture.SetImageFile(NULL);
        m_btnSubscribe.SetWindowText(ConvertString("Subscribe"));
        Invalidate(TRUE);
    }
    else
    { 
        int nChannel = m_cmbChannel.GetCurSel();
        m_lRealLoadHandle = CLIENT_RealLoadPictureEx(m_lLoginHandle, nChannel, EVENT_IVS_ALL, TRUE, CbIntelligentEvent, (LDWORD)this, NULL);
        if (m_lRealLoadHandle == 0)
        {		
            MessageBox(ConvertString("Subscribe intelligent failed!"), ConvertString("Prompt"));
            return;
        }
        m_btnSubscribe.SetWindowText(ConvertString("Unsubscribe"));
    }
}

LRESULT CIntelligentEventDlg::OnIntelligentEvent(WPARAM wParam, LPARAM lParam)
{
    if (0 == wParam)
    {
        return 0;
    }

    TRAFFIC_INTELLIGENT_EVENT_INFO* pEventInfo = (TRAFFIC_INTELLIGENT_EVENT_INFO*)wParam;

    // Show intelligent event info in list control
    ShowIntelligentEvnetInfo(pEventInfo);
    delete[] pEventInfo->pBuf;
    delete pEventInfo;
    return 0;
}

// Self-define function WM_INTELLIGENT_EVENT
void CIntelligentEventDlg::ShowIntelligentEvnetInfo( TRAFFIC_INTELLIGENT_EVENT_INFO * pTrafficEvent )
{
    if (m_ctrEventList.GetItemCount() > MAX_EVENT_IN_LIST_CONTROL)
    {
        m_ctrEventList.DeleteItem(MAX_EVENT_IN_LIST_CONTROL);
    }

    // Snap Picture
    m_ctrEventPicture.SetImageDate(pTrafficEvent->pBuf, pTrafficEvent->dwBufSize);

    // Plate Picture
    if (0 == strlen(pTrafficEvent->cPlateNumber))
    {
        m_ctrPlatePicture.SetImageFile(NULL);
        Invalidate();
    }
    else
    {
        int length = pTrafficEvent->dwOffSet+pTrafficEvent->dwFileLength;
        if (pTrafficEvent->dwFileLength > 0 && length<= pTrafficEvent->dwBufSize)
        {
            m_ctrPlatePicture.SetImageDate(pTrafficEvent->pBuf + pTrafficEvent->dwOffSet, pTrafficEvent->dwFileLength);
        }
    }

    CString strIndex;
    CString strLane;
    CString strTime;
    CString strSpeed;

    m_nIndexOfEvent++;
    strIndex.Format("%d",m_nIndexOfEvent);	
    strLane.Format("%d", pTrafficEvent->nLane);
    strSpeed.Format("%d", pTrafficEvent->nSpeed);
    strTime.Format("%04d-%02d-%02d %02d:%02d:%02d",
        pTrafficEvent->szTime.dwYear, pTrafficEvent->szTime.dwMonth, 
        pTrafficEvent->szTime.dwDay, pTrafficEvent->szTime.dwHour, 
        pTrafficEvent->szTime.dwMinute, pTrafficEvent->szTime.dwSecond);

    LV_ITEM lvi;
    lvi.mask=LVIF_TEXT|LVIF_IMAGE|LVIF_PARAM;
    lvi.iSubItem = 0;
    lvi.pszText = _T("");
    lvi.iImage = 0;
    lvi.iItem = 0;

    m_ctrEventList.InsertItem(&lvi);
    m_ctrEventList.SetItemText(0,0, strIndex);
    m_ctrEventList.SetItemText(0,1, strTime);
    m_ctrEventList.SetItemText(0,2, ConvertString(pTrafficEvent->cEventName));
    m_ctrEventList.SetItemText(0,3, strLane);
    m_ctrEventList.SetItemText(0,4, pTrafficEvent->cPlateNumber);
    m_ctrEventList.SetItemText(0,5, pTrafficEvent->cPlateColor);
    m_ctrEventList.SetItemText(0,6, pTrafficEvent->cVehicleColor);
    m_ctrEventList.SetItemText(0,7, strSpeed);
    m_ctrEventList.SetItemText(0,8, pTrafficEvent->cVehicleSign);
}
