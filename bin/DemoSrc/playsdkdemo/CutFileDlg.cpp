// CutFileDlg.cpp : implementation file
//

#include "stdafx.h"
#include "PlayDemo.h"
#include "CutFileDlg.h"
#include "Player.h"
#include "LanguageConvertor.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CCutFileDlg dialog


CCutFileDlg::CCutFileDlg(CString originfile,CWnd* pParent /*=NULL*/)
	: CDialog(CCutFileDlg::IDD, pParent)
{
	//{{AFX_DATA_INIT(CCutFileDlg)
	m_cutType = 0;
	m_realStartPos = 0;
	m_endpos = 0;
	m_originfile = originfile;
	m_realStartPos = 0;
	m_realEndPos = 0;
	m_startpos = 0;
	//}}AFX_DATA_INIT
}


void CCutFileDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CCutFileDlg)
	DDX_Radio(pDX, IDC_RADIO_FRAME, m_cutType);
	DDX_Text(pDX, IDC_EDIT_STARTPOS, m_startpos);
	DDX_Text(pDX, IDC_EDIT_ENDPOS, m_endpos);
	DDX_Text(pDX, IDC_EDIT_REALSTART, m_realStartPos);
	DDX_Text(pDX, IDC_EDIT_REALEND, m_realEndPos);
	//}}AFX_DATA_MAP
}

BOOL CCutFileDlg::OnInitDialog()
{
	CDialog::OnInitDialog();
	
	LANG_SETWNDSTATICTEXT(this);

	//get range of the framenum and time.
	m_nMaxTime=CPlayer::Instance()->GetTotalTime();
	m_nMaxFrameNum=	CPlayer::Instance()->GetTotalFrame();
	CString s_range;
	s_range.Format(_T("%s:         %d--%d\r\n%s(Sec):     %d--%d\r\n"),
		LANG_CS("Frame Range"), 0, m_nMaxFrameNum -1, LANG_CS("Time Range"), 0, m_nMaxTime);
	GetDlgItem(IDC_RANGEVALUE)->SetWindowText(s_range);

	return TRUE;
}

BEGIN_MESSAGE_MAP(CCutFileDlg, CDialog)
	//{{AFX_MSG_MAP(CCutFileDlg)
	ON_BN_CLICKED(IDC_BUTTON_SAVE, OnButtonSave)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CCutFileDlg message handlers

void CCutFileDlg::OnButtonSave() 
{
	// TODO: Add your control notification handler code here
	CString strEdit1,strEdit2;
	GetDlgItemTextW(IDC_EDIT_STARTPOS,strEdit1);
	GetDlgItemTextW(IDC_EDIT_ENDPOS,strEdit2);
	if(strEdit1.IsEmpty() || strEdit2.IsEmpty())
	{
		AfxMessageBox(LANG_CS("Please enter an integer."));
		return;
	}

	if (UpdateData(true) == 0)
		return;

	int startPos = m_startpos;
	int endPos = m_endpos;

	m_cutType = ((CButton*)GetDlgItem(IDC_RADIO_FRAME))->GetCheck()?CUTBYFRAMENUM:CUTBYTIME;
	if (m_cutType == CUTBYFRAMENUM)  //By Frame number
	{
		if (startPos < 0 || startPos > m_nMaxFrameNum -1
			|| endPos< 0 || endPos > m_nMaxFrameNum -1
			|| startPos > endPos)
		{
			AfxMessageBox(LANG_CS("Input number error!"));
			return ;
		}
		else
		{
			/*To unify frame no. and time, the frame no. 
			starts from 0 and time starts from 0; frame no.
			requires operation before unified use of GetKeyFramePos and GetNextKeyFramePos port*/
		}
	}
	else
	{
		if (startPos < 0|| startPos > m_nMaxTime
			|| endPos < 0|| endPos > m_nMaxTime
			|| startPos > endPos)
		{
			AfxMessageBox(LANG_CS("Input number error!"));
			return;
		}
	}

	CPlayer::Instance()->GetKeyFramePos(startPos*((m_cutType == 0)?1:1000), m_cutType+1, &m_RealBegin);
	CPlayer::Instance()->GetNextKeyFramePos(endPos*((m_cutType == 0)?1:1000), m_cutType+1, &m_RealEnd);

	LONGLONG newfilelen = 0;
	CFile file_in(m_originfile,CFile::modeRead|CFile::typeBinary|CFile::shareDenyNone);
	if (m_RealBegin.nFrameNum > m_RealEnd.nFrameNum)
	{
		AfxMessageBox(LANG_CS("Input number error!"));
		return ;
	}
	else if((m_cutType == 0 && (m_RealEnd.nFrameNum) < endPos) ||
		(m_cutType == 1 && m_RealEnd.nFrameTime < ((endPos)*1000)))
	{
		// This situation usually happens when cut last few frames.
		newfilelen = file_in.GetLength() - m_RealBegin.nFilePos;
		m_realEndPos = (m_cutType == 0)?(CPlayer::Instance()->GetTotalFrame()-1):(CPlayer::Instance()->GetTotalTime());
	}
	else
	{
		m_realEndPos = (m_cutType == 0)?(m_RealEnd.nFrameNum):((m_RealEnd.nFrameTime/1000));
		newfilelen = m_RealEnd.nFilePos+ m_RealEnd.nFrameLen - m_RealBegin.nFilePos ;
	}
	m_realStartPos = (m_cutType == 0)?(m_RealBegin.nFrameNum):(m_RealBegin.nFrameTime/1000); 

	TCHAR szFilters[]= _T("dav Files (*.dav)|*.dav|All Files (*.*)|*.*||");
	CFileDialog FileChooser (FALSE, _T("dav"), _T("*.dav"), OFN_FILEMUSTEXIST,szFilters, this);
	CString savefile ;
	if (FileChooser.DoModal()==IDOK)
	{
		savefile = FileChooser.GetPathName();
		UpdateData(FALSE);
	}
	else 
	{
		UpdateData(FALSE);
		return;
	}

	/*Copy portion that is to be cut into new file*/
	CFile file_out(savefile,CFile::modeWrite|CFile::typeBinary|CFile::modeCreate);
	
	const int nSize = 2048;
	int nBlock = newfilelen/nSize;
	char *pBuf = new char[nSize];
	memset(pBuf, 0, nSize);

	if (m_RealBegin.nFilePos < (1<<31))
		file_in.Seek(m_RealBegin.nFilePos,SEEK_SET);
	else
	{
		DWORD zgf = file_in.Seek(m_RealBegin.nFilePos/2,SEEK_SET);
		zgf = file_in.Seek(m_RealBegin.nFilePos - m_RealBegin.nFilePos/2,SEEK_CUR);
	}
	
	for (DWORD i = 0 ; i < nBlock ; i++ )
	{
		file_in.Read(pBuf,nSize);
		file_out.Write(pBuf,nSize);
	}
	DWORD nRemain = newfilelen - nBlock*nSize;
	if (nRemain > 0)
	{
		file_in.Read(pBuf,nRemain);
		file_out.Write(pBuf,nRemain);
	}
	delete[] pBuf;
	file_out.Close();	
}

BOOL CCutFileDlg::PreTranslateMessage(MSG* pMsg) 
{
	// TODO: Add your specialized code here and/or call the base class

	/*Prevent user to close the box by pressing enter */
	if (pMsg->wParam == VK_RETURN && pMsg->message == WM_KEYDOWN)
	{
		OnButtonSave();
		return 1;
	}
	else if(pMsg->message == WM_KEYDOWN && pMsg->wParam == VK_ESCAPE)
		return 1;
	return CDialog::PreTranslateMessage(pMsg);
}
