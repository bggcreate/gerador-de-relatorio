// AddAndModifyBWDlg.cpp : implementation file
//

#include "stdafx.h"
#include "IntelligentTraffic.h"
#include "AddAndModifyBWDlg.h"


// CAddAndModifyBWDlg Dialog

IMPLEMENT_DYNAMIC(CAddAndModifyBWDlg, CDialog)

CAddAndModifyBWDlg::CAddAndModifyBWDlg(CWnd* pParent /*=NULL*/)
	: CDialog(CAddAndModifyBWDlg::IDD, pParent)
{
    m_lLoginHandle = 0;
    memset(&m_stTrafficListInfo, 0, sizeof(NET_TRAFFIC_LIST_RECORD));
    m_stTrafficListInfo.dwSize = sizeof(NET_TRAFFIC_LIST_RECORD);
    m_emTrafficOperatorType = EM_ADD_BACKLIST;
}

CAddAndModifyBWDlg::~CAddAndModifyBWDlg()
{
}

void CAddAndModifyBWDlg::DoDataExchange(CDataExchange* pDX)
{
    CDialog::DoDataExchange(pDX);
}


BEGIN_MESSAGE_MAP(CAddAndModifyBWDlg, CDialog)
    ON_BN_CLICKED(IDC_BTN_OK, &CAddAndModifyBWDlg::OnBnClickedBtnOk)
    ON_BN_CLICKED(IDC_BTN_CANCLE, &CAddAndModifyBWDlg::OnBnClickedBtnCancle)
    ON_NOTIFY(DTN_DATETIMECHANGE, IDC_DATETIMEPICKER_START_DATE, &CAddAndModifyBWDlg::OnDtnDatetimechangeDatetimepickerStartDate)
    ON_NOTIFY(DTN_DATETIMECHANGE, IDC_DATETIMEPICKER_END_DATE, &CAddAndModifyBWDlg::OnDtnDatetimechangeDatetimepickerEndDate)
END_MESSAGE_MAP()


// CAddAndModifyBWDlg message handlers

BOOL CAddAndModifyBWDlg::PreTranslateMessage(MSG* pMsg)
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

BOOL CAddAndModifyBWDlg::OnInitDialog()
{
    CDialog::OnInitDialog();
    g_SetWndStaticText(this);

    SetDlgItemText(IDC_EDIT_PLATE_NUMBER,m_stTrafficListInfo.szPlateNumber);
    SetDlgItemText(IDC_EDIT_OWNER,m_stTrafficListInfo.szMasterOfCar);

    CString strWndindowName;
    switch (m_emTrafficOperatorType)
    {
    case EM_ADD_BACKLIST:
        strWndindowName = ConvertString("Add Black List");
        break;
    case EM_ADD_WHITELIST:
        strWndindowName = ConvertString("Add White List");
        break;
    case EM_MODIFY_BACKLIST:
        ShowTime();
        strWndindowName = ConvertString("Modify Black List");
        break;
    case EM_MODIFY_WHITELIST:
        ShowTime();
        strWndindowName = ConvertString("Modify White List");
        break;
    default:
        strWndindowName = ConvertString("Unknown");
        break;
    }
    SetWindowText(strWndindowName);
    return TRUE;  // return TRUE unless you set the focus to a control
}


void CAddAndModifyBWDlg::SetLoginHandle(const LLONG lLoginHandle)
{
    if (0 != lLoginHandle)
    {
        m_lLoginHandle = lLoginHandle;
    }
}


void CAddAndModifyBWDlg::OnBnClickedBtnOk()
{
    if (0 == m_lLoginHandle)
    {
        return;
    }
    bool bRet = IsTimeCorrent();
    if (!bRet)
    {
        MessageBox(ConvertString("The begin time is bigger than end time, please input again!"), ConvertString("Prompt"));
        return;
    }

    CString strPlateNumber;
    CString strOwner;

    GetDlgItemText(IDC_EDIT_PLATE_NUMBER, strPlateNumber);
    GetDlgItemText(IDC_EDIT_OWNER, strOwner);

    NET_IN_OPERATE_TRAFFIC_LIST_RECORD stInParam = { sizeof(NET_IN_OPERATE_TRAFFIC_LIST_RECORD) };
    NET_OUT_OPERATE_TRAFFIC_LIST_RECORD stOutParam = { sizeof(NET_OUT_OPERATE_TRAFFIC_LIST_RECORD) };
    CDateTimeCtrl* pCtrBeginDate = (CDateTimeCtrl*)GetDlgItem(IDC_DATETIMEPICKER_START_DATE);
    CDateTimeCtrl* pCtrBeginTime = (CDateTimeCtrl*)GetDlgItem(IDC_DATETIMEPICKER_START_TIME);
    CDateTimeCtrl* pCtrEndtDate = (CDateTimeCtrl*)GetDlgItem(IDC_DATETIMEPICKER_END_DATE);
    CDateTimeCtrl* pCtrEndTime = (CDateTimeCtrl*)GetDlgItem(IDC_DATETIMEPICKER_END_TIME);
    if (EM_ADD_BACKLIST == m_emTrafficOperatorType || EM_ADD_WHITELIST == m_emTrafficOperatorType)
    {
        stInParam.emOperateType = NET_TRAFFIC_LIST_INSERT;
        if (EM_ADD_BACKLIST == m_emTrafficOperatorType )
        {
            stInParam.emRecordType = NET_RECORD_TRAFFICBLACKLIST;
        }
        else
        {
            stInParam.emRecordType = NET_RECORD_TRAFFICREDLIST;
        }

        NET_TRAFFIC_LIST_RECORD stTrafficListRecord = { sizeof(NET_TRAFFIC_LIST_RECORD) };
        GetTimeFromTimeCtr(stTrafficListRecord.stBeginTime, pCtrBeginDate, pCtrBeginTime);
        GetTimeFromTimeCtr(stTrafficListRecord.stCancelTime, pCtrEndtDate, pCtrEndTime);
        strncpy(stTrafficListRecord.szPlateNumber, strPlateNumber.GetBuffer(), DH_MAX_PLATE_NUMBER_LEN-1);
        strncpy(stTrafficListRecord.szMasterOfCar, strOwner.GetBuffer(), DH_MAX_NAME_LEN-1);  

        NET_INSERT_RECORD_INFO stInsertInfo = { sizeof( NET_INSERT_RECORD_INFO) };
        stInsertInfo.pRecordInfo = &stTrafficListRecord;
        stInParam.pstOpreateInfo = &stInsertInfo;
        BOOL bRet = CLIENT_OperateTrafficList(m_lLoginHandle, &stInParam, &stOutParam, MAX_TIMEOUT);
        if (!bRet)
        {
            MessageBox(ConvertString("Operate Traffic list failed!"), ConvertString("Prompt"));
            return;
        }
    }
    else
    {
        stInParam.emOperateType = NET_TRAFFIC_LIST_UPDATE;
        if (EM_MODIFY_BACKLIST == m_emTrafficOperatorType)
        {
            stInParam.emRecordType = NET_RECORD_TRAFFICBLACKLIST;
        }
        else
        {
            stInParam.emRecordType = NET_RECORD_TRAFFICREDLIST;
        }
        NET_TRAFFIC_LIST_RECORD stTrafficListRecord = { sizeof(NET_TRAFFIC_LIST_RECORD) };
        GetTimeFromTimeCtr(stTrafficListRecord.stBeginTime, pCtrBeginDate, pCtrBeginTime);
        GetTimeFromTimeCtr(stTrafficListRecord.stCancelTime, pCtrEndtDate, pCtrEndTime);
        strncpy(stTrafficListRecord.szPlateNumber, strPlateNumber.GetBuffer(), DH_MAX_PLATE_NUMBER_LEN-1);
        strncpy(stTrafficListRecord.szMasterOfCar, strOwner.GetBuffer(), DH_MAX_NAME_LEN-1);
        stTrafficListRecord.nRecordNo = m_stTrafficListInfo.nRecordNo;  //¼ÇÂ¼¼¯±àºÅ

        NET_UPDATE_RECORD_INFO stModifyRecord = {sizeof(NET_UPDATE_RECORD_INFO)};
        stModifyRecord.pRecordInfo = &stTrafficListRecord;
        stInParam.pstOpreateInfo = &stModifyRecord;
        BOOL bRet = CLIENT_OperateTrafficList(m_lLoginHandle, &stInParam, &stOutParam, MAX_TIMEOUT);
        if (!bRet)
        {
            MessageBox(ConvertString("Operate Traffic list failed!"), ConvertString("Prompt"));
            return;
        }
    }
   
    OnOK();
}

void CAddAndModifyBWDlg::OnBnClickedBtnCancle()
{
    OnCancel();
}


void CAddAndModifyBWDlg::GetTimeFromTimeCtr(NET_TIME& stTime, CDateTimeCtrl* pCtrDate, CDateTimeCtrl* pCtrTime)
{
    if (NULL == pCtrDate || NULL == pCtrTime)
    {
        return;
    }

    COleDateTime tmDate;
    COleDateTime tmTime;
    pCtrDate->GetTime(tmDate);
    pCtrTime->GetTime(tmTime);

    stTime.dwYear = tmDate.GetYear();
    stTime.dwMonth = tmDate.GetMonth();
    stTime.dwDay = tmDate.GetDay();
    stTime.dwHour = tmTime.GetHour();
    stTime.dwMinute = tmTime.GetMinute();
    stTime.dwSecond = tmTime.GetSecond();
}


void CAddAndModifyBWDlg::SetTrafficListInfo(const NET_TRAFFIC_LIST_RECORD* pTrafficListInfo)
{
    if (NULL != pTrafficListInfo)
    {
        memcpy(&m_stTrafficListInfo, pTrafficListInfo, sizeof(NET_TRAFFIC_LIST_RECORD));
    }
}


void CAddAndModifyBWDlg::SetTrafficOperatorType(EM_TRAFFIC_LIST_OPERATOR_TYPE emTrafficOperatorType)
{
    m_emTrafficOperatorType = emTrafficOperatorType;
}

void CAddAndModifyBWDlg::ShowTime()
{
    COleDateTime tmDate;
    tmDate.SetDate(m_stTrafficListInfo.stBeginTime.dwYear, m_stTrafficListInfo.stBeginTime.dwMonth, m_stTrafficListInfo.stBeginTime.dwDay);
    ((CDateTimeCtrl*)GetDlgItem(IDC_DATETIMEPICKER_START_DATE))->SetTime(tmDate);

    tmDate.SetDate(m_stTrafficListInfo.stCancelTime.dwYear, m_stTrafficListInfo.stCancelTime.dwMonth, m_stTrafficListInfo.stCancelTime.dwDay);
    ((CDateTimeCtrl*)GetDlgItem(IDC_DATETIMEPICKER_END_DATE))->SetTime(tmDate);

    tmDate.SetTime(m_stTrafficListInfo.stBeginTime.dwHour, m_stTrafficListInfo.stBeginTime.dwMinute, m_stTrafficListInfo.stBeginTime.dwSecond);
    ((CDateTimeCtrl*)GetDlgItem(IDC_DATETIMEPICKER_START_TIME))->SetTime(tmDate);

    tmDate.SetTime(m_stTrafficListInfo.stCancelTime.dwHour, m_stTrafficListInfo.stCancelTime.dwMinute, m_stTrafficListInfo.stCancelTime.dwSecond);
    ((CDateTimeCtrl*)GetDlgItem(IDC_DATETIMEPICKER_END_TIME))->SetTime(tmDate);
}

bool CAddAndModifyBWDlg::IsTimeCorrent()
{
    CDateTimeCtrl* pCtrBeginDate = (CDateTimeCtrl*)GetDlgItem(IDC_DATETIMEPICKER_START_DATE);
    CDateTimeCtrl* pCtrBeginTime = (CDateTimeCtrl*)GetDlgItem(IDC_DATETIMEPICKER_START_TIME);
    CDateTimeCtrl* pCtrEndtDate = (CDateTimeCtrl*)GetDlgItem(IDC_DATETIMEPICKER_END_DATE);
    CDateTimeCtrl* pCtrEndTime = (CDateTimeCtrl*)GetDlgItem(IDC_DATETIMEPICKER_END_TIME);

    COleDateTime beginDate;
    COleDateTime beginTime;
    COleDateTime endDate;
    COleDateTime endTime;
    pCtrBeginDate->GetTime(beginDate);
    pCtrBeginTime->GetTime(beginTime);
    pCtrEndtDate->GetTime(endDate);
    pCtrEndTime->GetTime(endTime);

    if (endDate >= beginDate)
    {
        if (endDate == beginDate)
        {
            if (endTime < beginTime)
            {
                return false;
            }
        }
    }
    else
    {
        return false;
    }
    return true;
}


void CAddAndModifyBWDlg::OnDtnDatetimechangeDatetimepickerStartDate(NMHDR *pNMHDR, LRESULT *pResult)
{
    LPNMDATETIMECHANGE pDTChange = reinterpret_cast<LPNMDATETIMECHANGE>(pNMHDR);

    if (2000 > pDTChange->st.wYear || 2038 < pDTChange->st.wYear)
    {
         MessageBox(ConvertString("The time range is from 2000 to 2038.Please input again!"), ConvertString("Prompt"));
        COleDateTime tmDate  = COleDateTime::GetCurrentTime();
        ((CDateTimeCtrl*)GetDlgItem(IDC_DATETIMEPICKER_START_DATE))->SetTime(tmDate);

    }   
    *pResult = 0;
}

void CAddAndModifyBWDlg::OnDtnDatetimechangeDatetimepickerEndDate(NMHDR *pNMHDR, LRESULT *pResult)
{
    LPNMDATETIMECHANGE pDTChange = reinterpret_cast<LPNMDATETIMECHANGE>(pNMHDR);
    if (2000 > pDTChange->st.wYear || 2038 < pDTChange->st.wYear)
    {
        MessageBox(ConvertString("The time range is from 2000 to 2038.Please input again!"), ConvertString("Prompt"));
        COleDateTime tmDate  = COleDateTime::GetCurrentTime();
        ((CDateTimeCtrl*)GetDlgItem(IDC_DATETIMEPICKER_END_DATE))->SetTime(tmDate);

    }   
    *pResult = 0;
}
