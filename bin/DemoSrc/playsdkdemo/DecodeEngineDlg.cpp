// DecodeEngineDlg.cpp : 实现文件
//

#include "stdafx.h"
#include "PlayDemo.h"
#include "DecodeEngineDlg.h"


// DecodeEngineDlg 对话框

IMPLEMENT_DYNAMIC(DecodeEngineDlg, CDialog)

DecodeEngineDlg::DecodeEngineDlg(CWnd* pParent /*=NULL*/)
	: CDialog(DecodeEngineDlg::IDD, pParent)
	,m_nType(0)
{

}

DecodeEngineDlg::~DecodeEngineDlg()
{
}

void DecodeEngineDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
}

BOOL DecodeEngineDlg::OnInitDialog() 
{
	CDialog::OnInitDialog();

	// TODO: Add extra initialization here
	((CButton*)GetDlgItem(IDC_RADIO_DECODE_0))->SetCheck(m_nType == 1);
	((CButton*)GetDlgItem(IDC_RADIO_DECODE_1))->SetCheck(m_nType == 3);

	//UpdateData(FALSE);

	//LANG_SETWNDSTATICTEXT(this);

	return TRUE;  // return TRUE unless you set the focus to a control
	// EXCEPTION: OCX Property Pages should return FALSE
}BEGIN_MESSAGE_MAP(DecodeEngineDlg, CDialog)
ON_BN_CLICKED(IDOK, OnBnClickedOk)
END_MESSAGE_MAP()

void DecodeEngineDlg::OnBnClickedOk()
{
	// TODO: 在此添加控件通知处理程序代码

	if(((CButton*)GetDlgItem(IDC_RADIO_DECODE_0))->GetCheck() != 0)
		m_nType = 1; //DECODE_SW
	else if(((CButton*)GetDlgItem(IDC_RADIO_DECODE_1))->GetCheck() != 0)
		m_nType = 3;//DECODE_HW_FAST

	OnOK();
}
