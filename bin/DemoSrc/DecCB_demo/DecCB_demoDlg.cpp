// DecCB_demoDlg.cpp : implementation file
//

#include "stdafx.h"
#include "DecCB_demo.h"
#include "DecCB_demoDlg.h"
#include "dhplay.h"
#include "LanguageConvertor.h"
#include <shlwapi.h>
#include "CharactorTansfer.h"
#include <string>


#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CDecCB_demoDlg dialog
#pragma comment(lib, "shlwapi.lib")
#pragma comment(lib, "dhplay.lib")

CDecCB_demoDlg::CDecCB_demoDlg(CWnd* pParent /*=NULL*/)
	: CDialog(CDecCB_demoDlg::IDD, pParent)
{
	//{{AFX_DATA_INIT(CDecCB_demoDlg)
	m_csSrcFilePath = _T("");
	m_csAudiofilePath = _T("");
	m_csVideofilePath = _T("");
	m_pOutVideoFile = NULL;
	m_pOutAudioFile = NULL;
	m_nDecType = 0;
	m_nInterfaceType = 0;
	//}}AFX_DATA_INIT
	// Note that LoadIcon does not require a subsequent DestroyIcon in Win32
	m_hIcon = AfxGetApp()->LoadIcon(IDR_MAINFRAME);
	LANG_INIT();
}

void CDecCB_demoDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CDecCB_demoDlg)
	DDX_Text(pDX, IDC_EDIT_FILEPATH, m_csSrcFilePath);
	DDX_Text(pDX, IDC_EDIT_AUDIOFILEPATH, m_csAudiofilePath);
	DDX_Text(pDX, IDC_EDIT_VIDEOFILEPATH, m_csVideofilePath);
	//}}AFX_DATA_MAP
}

BEGIN_MESSAGE_MAP(CDecCB_demoDlg, CDialog)
	//{{AFX_MSG_MAP(CDecCB_demoDlg)
	ON_WM_PAINT()
	ON_WM_QUERYDRAGICON()
	ON_BN_CLICKED(IDC_BUTTON_FILEPATH, OnButtonFilepath)
	ON_BN_CLICKED(IDC_BUTTON_DECODE, OnButtonDecode)
	ON_BN_CLICKED(IDC_BUTTON_STOP, OnButtonStop)
	ON_BN_CLICKED(IDC_RADIO_AUDIO, OnRadioAudio)
	ON_BN_CLICKED(IDC_RADIO_COMPLEX, OnRadioComplex)
	ON_BN_CLICKED(IDC_RADIO_VIDEO, OnRadioVideo)
	ON_WM_CLOSE()
	ON_BN_CLICKED(IDC_BUTTON_AUDIOFILE, OnButtonAudiofile)
	ON_BN_CLICKED(IDC_BUTTON_VIDEOFILE, OnButtonVideofile)
	ON_BN_CLICKED(IDC_INTERFACE_NEW, OnInterfaceNew)
	ON_BN_CLICKED(IDC_INTERFACE_OLD, OnInterfaceOld)
	ON_BN_CLICKED(IDC_INTERFACE_NEW2, OnInterfaceNew2)
	ON_BN_CLICKED(IDC_INTERFACE_OLD2, OnInterfaceOld2)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CDecCB_demoDlg message handlers

BOOL CDecCB_demoDlg::OnInitDialog()
{
	CDialog::OnInitDialog();

	// Set the icon for this dialog.  The framework does this automatically
	//  when the application's main window is not a dialog
	SetIcon(m_hIcon, TRUE);			// Set big icon
	SetIcon(m_hIcon, FALSE);		// Set small icon
	
	// TODO: Add extra initialization here
	
	LANG_SETWNDSTATICTEXT(this);
	
	//init ui
	((CButton*)GetDlgItem(IDC_RADIO_VIDEO))->SetCheck(BST_CHECKED);
	((CButton*)GetDlgItem(IDC_INTERFACE_NEW))->SetCheck(BST_CHECKED);
	m_displayDlg.Create(IDD_DIALOG_DISPLAY, this);

	ChangeUIState(STOPDEC);
	return TRUE;  // return TRUE  unless you set the focus to a control
}

// If you add a minimize button to your dialog, you will need the code below
//  to draw the icon.  For MFC applications using the document/view model,
//  this is automatically done for you by the framework.

void CDecCB_demoDlg::OnPaint() 
{
	if (IsIconic())
	{
		CPaintDC dc(this); // device context for painting

		SendMessage(WM_ICONERASEBKGND, (WPARAM) dc.GetSafeHdc(), 0);

		// Center icon in client rectangle
		int cxIcon = GetSystemMetrics(SM_CXICON);
		int cyIcon = GetSystemMetrics(SM_CYICON);
		CRect rect;
		GetClientRect(&rect);
		int x = (rect.Width() - cxIcon + 1) / 2;
		int y = (rect.Height() - cyIcon + 1) / 2;

		// Draw the icon
		dc.DrawIcon(x, y, m_hIcon);
	}
	else
	{
		CDialog::OnPaint();
	}
}

// The system calls this to obtain the cursor to display while the user drags
//  the minimized window.
HCURSOR CDecCB_demoDlg::OnQueryDragIcon()
{
	return (HCURSOR) m_hIcon;
}

void CDecCB_demoDlg::OnButtonFilepath() 
{
	// TODO: Add your control notification handler code here
	CFileDialog filechooser(TRUE, NULL, NULL, OFN_HIDEREADONLY | OFN_OVERWRITEPROMPT, _T("All files(*.*)|*.*||"));
	if(filechooser.DoModal() == IDOK)
		m_csSrcFilePath = filechooser.GetPathName();
	GetDlgItem(IDC_EDIT_FILEPATH)->SetWindowText(m_csSrcFilePath);

	m_csAudiofilePath = m_csSrcFilePath;
	m_csAudiofilePath.Replace(CString(".")+filechooser.GetFileExt(), _T(".pcm"));
	GetDlgItem(IDC_EDIT_AUDIOFILEPATH)->SetWindowText(m_csAudiofilePath);

	m_csVideofilePath = m_csSrcFilePath;
	m_csVideofilePath.Replace(CString(".")+filechooser.GetFileExt(), _T(".yuv"));
	GetDlgItem(IDC_EDIT_VIDEOFILEPATH)->SetWindowText(m_csVideofilePath);
}

void CALLBACK DecCbFun(long nPort, char *pBuf, long nSize, FRAME_INFO * pFrameInfo, void* pUserData, long nReserved2)
{
	CDecCB_demoDlg *dlg = (CDecCB_demoDlg *)pUserData;
	
	if(pFrameInfo->nType == T_AUDIO16 || pFrameInfo->nType==T_AUDIO8)
		fwrite(pBuf, 1, nSize, dlg->m_pOutAudioFile);
	
	if(pFrameInfo->nType == T_IYUV)
		fwrite(pBuf, 1, nSize, dlg->m_pOutVideoFile);
}

void CALLBACK cbDecode(long nPort, FRAME_DECODE_INFO* pFrameDecodeInfo, FRAME_INFO_EX* pFrameInfo, void* pUser)
{
	CDecCB_demoDlg *dlg = (CDecCB_demoDlg *)pUser;
	if(pFrameDecodeInfo->nFrameType==FRAME_TYPE_VIDEO && dlg->m_pOutVideoFile)
	{
		for(int nIndex=0; nIndex<3; nIndex++)
		{
			char* pData = (char*)pFrameDecodeInfo->pVideoData[nIndex];
			for(int i=0; i<pFrameDecodeInfo->nHeight[nIndex]; i++)
			{
				size_t len = fwrite(pData, 1, pFrameDecodeInfo->nWidth[nIndex], dlg->m_pOutVideoFile);
				pData += pFrameDecodeInfo->nStride[nIndex];
			}
		}
	}

	if(pFrameDecodeInfo->nFrameType==FRAME_TYPE_AUDIO && dlg->m_pOutAudioFile)
	{
		fwrite(pFrameDecodeInfo->pAudioData, 1, pFrameDecodeInfo->nAudioDataLen, dlg->m_pOutAudioFile);
	}
}

void CALLBACK cbVisibleDec(long nPort,char * pBuf,long nSize,FRAME_INFO * pFrameInfo, void* pUserData, long nReserved2)
{
	CDecCB_demoDlg *dlg = (CDecCB_demoDlg *)pUserData;
	if(dlg->m_nDecType!=CDecCB_demoDlg::DEC_VIDEO 
		&& (pFrameInfo->nType == T_AUDIO16 || pFrameInfo->nType==T_AUDIO8) )
		fwrite(pBuf, 1, nSize, dlg->m_pOutAudioFile);
	
	if(dlg->m_nDecType!=CDecCB_demoDlg::DEC_AUDIO 
		&& pFrameInfo->nType == T_IYUV)
		fwrite(pBuf, 1, nSize, dlg->m_pOutVideoFile);
}

void CALLBACK fileEndCBFun(DWORD nPort, void* pUserData)
{
	CDecCB_demoDlg *dlg = (CDecCB_demoDlg *)pUserData;
	dlg->PostMessage(WM_COMMAND, IDC_BUTTON_STOP, 0);
}

void CDecCB_demoDlg::ChangeUIState(int nState)
{
	if (nState == STARTDEC)
	{
		GetDlgItem(IDC_BUTTON_STOP)->EnableWindow(TRUE);
		GetDlgItem(IDC_BUTTON_PLAY)->EnableWindow(FALSE);
		GetDlgItem(IDC_EDIT_FILEPATH)->EnableWindow(FALSE);
		GetDlgItem(IDC_EDIT_AUDIOFILEPATH)->EnableWindow(FALSE);
		GetDlgItem(IDC_EDIT_VIDEOFILEPATH)->EnableWindow(FALSE);
		GetDlgItem(IDC_BUTTON_FILEPATH)->EnableWindow(FALSE);
		GetDlgItem(IDC_BUTTON_VIDEOFILE)->EnableWindow(FALSE);
		GetDlgItem(IDC_BUTTON_AUDIOFILE)->EnableWindow(FALSE);
		GetDlgItem(IDC_RADIO_VIDEO)->EnableWindow(FALSE);
		GetDlgItem(IDC_RADIO_AUDIO)->EnableWindow(FALSE);
		GetDlgItem(IDC_RADIO_COMPLEX)->EnableWindow(FALSE);
		GetDlgItem(IDC_INTERFACE_NEW)->EnableWindow(FALSE);
		GetDlgItem(IDC_INTERFACE_OLD)->EnableWindow(FALSE);
		GetDlgItem(IDC_INTERFACE_NEW2)->EnableWindow(FALSE);
		GetDlgItem(IDC_INTERFACE_OLD2)->EnableWindow(FALSE);
	}
	if (nState == STOPDEC)
	{
		GetDlgItem(IDC_BUTTON_STOP)->EnableWindow(FALSE);
		GetDlgItem(IDC_BUTTON_PLAY)->EnableWindow(TRUE);
		GetDlgItem(IDC_EDIT_FILEPATH)->EnableWindow(TRUE);
		GetDlgItem(IDC_BUTTON_FILEPATH)->EnableWindow(TRUE);
		GetDlgItem(IDC_BUTTON_VIDEOFILE)->EnableWindow(TRUE);
		GetDlgItem(IDC_BUTTON_AUDIOFILE)->EnableWindow(TRUE);
		GetDlgItem(IDC_EDIT_AUDIOFILEPATH)->EnableWindow(TRUE);
		GetDlgItem(IDC_EDIT_VIDEOFILEPATH)->EnableWindow(TRUE);
		GetDlgItem(IDC_RADIO_VIDEO)->EnableWindow(TRUE);
		GetDlgItem(IDC_RADIO_AUDIO)->EnableWindow(TRUE);
		GetDlgItem(IDC_RADIO_COMPLEX)->EnableWindow(TRUE);
		GetDlgItem(IDC_INTERFACE_NEW)->EnableWindow(TRUE);
		GetDlgItem(IDC_INTERFACE_OLD)->EnableWindow(TRUE);
		GetDlgItem(IDC_INTERFACE_NEW2)->EnableWindow(TRUE);
		GetDlgItem(IDC_INTERFACE_OLD2)->EnableWindow(TRUE);
	}
}

void CDecCB_demoDlg::OnButtonDecode() 
{
	if (!UpdateData(TRUE))
	{
		return;
	}
	// TODO: Add your control notification handler code here

	//check the source file
	if (!PathFileExists(m_csSrcFilePath))
	{
		MessageBox(LANG_CS("Source file error!"));
		return;
	}

	//check the output file of YUV data
	if(m_nDecType != DEC_VIDEO)
	{
		m_pOutAudioFile = _tfopen(m_csAudiofilePath.GetBuffer(0), _T("wb+"));
		if(m_pOutAudioFile == NULL)
		{
			MessageBox(LANG_CS("Output audio file error!"));
			return;
		}
	}

	//check the output file of Audio data
	if (m_nDecType != DEC_AUDIO)
	{
		m_pOutVideoFile = _tfopen(m_csVideofilePath.GetBuffer(0), _T("wb+"));
		if(m_pOutVideoFile == NULL)
		{
			MessageBox(LANG_CS("Output YUV file error!"));
			return;
		}
	}

	std::string strFileNameA = UnicodeToGbk(m_csSrcFilePath.GetBuffer(0));
	if (!PLAY_OpenFile(0,(char*)strFileNameA.c_str()))
	{
		ChangeUIState(STOPDEC);
		MessageBox(LANG_CS("Open source file failed!"));
		fclose(m_pOutVideoFile);
		fclose(m_pOutAudioFile);
		return;
	}
	/*set the callback func of the end of file*/
	PLAY_SetFileEndCallBack(0, fileEndCBFun, this);
	
	/*0,1: decode callback,without video rendering*/
	if(m_nInterfaceType==0 || m_nInterfaceType==1) 
	{
		/*set callback type*/
		PLAY_SetDecCBStream(0, m_nDecType+1);
		/*set callback func.*/
		PLAY_SetFileEndCallBack(0, fileEndCBFun, this);
		/*set decode callback func. */
		if(m_nInterfaceType==0)
			PLAY_SetDecCallBackEx(0, DecCbFun, this);
		if(m_nInterfaceType==1)
			PLAY_SetDecodeCallBack(0, cbDecode, this);
		PLAY_Play(0, NULL);
	}

	/*2,3:decode callback with video rendering*/
	if(m_nInterfaceType==2 || m_nInterfaceType==3) 
	{
		if(m_nInterfaceType==2)
			PLAY_SetVisibleDecodeCallBack(0, cbDecode, this);	
		if(m_nInterfaceType==3)
			PLAY_SetVisibleDecCallBack(0, cbVisibleDec, this);
			
		
		m_displayDlg.ShowWindow(SW_NORMAL);
		PLAY_Play(0, m_displayDlg.m_hWnd);
		PLAY_PlaySound(0);
	}
	//start decode
	ChangeUIState(STARTDEC);
}

void CDecCB_demoDlg::OnButtonStop() 
{
	// TODO: Add your control notification handler code here

	//set callback which you choose to null
	PLAY_SetDecCallBackEx(0, NULL, NULL);
	PLAY_SetDecodeCallBack(0, NULL, NULL);
	PLAY_SetVisibleDecodeCallBack(0, NULL, NULL);	
	PLAY_SetVisibleDecCallBack(0, NULL, NULL);

	//stop decode
	PLAY_Stop(0);
	PLAY_CloseFile(0);

	//close file
	if(m_pOutVideoFile != NULL)
	{
		fclose(m_pOutVideoFile);
		m_pOutVideoFile = NULL;
	}
	if(m_pOutAudioFile != NULL)
	{
		fclose(m_pOutAudioFile);
		m_pOutAudioFile = NULL;
	}
	
	m_displayDlg.ShowWindow(SW_HIDE);

	ChangeUIState(STOPDEC);
}

void CDecCB_demoDlg::OnRadioAudio() 
{
	// TODO: Add your control notification handler code here
	m_nDecType = DEC_AUDIO;
}

void CDecCB_demoDlg::OnRadioComplex() 
{
	// TODO: Add your control notification handler code here
	m_nDecType = DEC_COMPLEX;
}

void CDecCB_demoDlg::OnRadioVideo() 
{
	// TODO: Add your control notification handler code here
	m_nDecType = DEC_VIDEO;
}

void CDecCB_demoDlg::OnClose() 
{
	// TODO: Add your message handler code here and/or call default
	OnButtonStop();
	CDialog::OnClose();
}

void CDecCB_demoDlg::OnButtonAudiofile() 
{
	// TODO: Add your control notification handler code here
	CString szTitle = LANG_CS("folder");
	TCHAR szDisplayName[MAX_PATH] =_T("");
	TCHAR szPath[MAX_PATH] =_T("");
	BROWSEINFO bi;
	bi.hwndOwner = GetSafeHwnd();
	bi.pidlRoot = NULL;
	bi.lpszTitle = szTitle;
	bi.pszDisplayName = szDisplayName;
	bi.ulFlags = BIF_RETURNONLYFSDIRS|BIF_BROWSEINCLUDEFILES;
	bi.lpfn = NULL;
	bi.lParam = 0;
	LPITEMIDLIST pItemIDList = SHBrowseForFolder(&bi);
	
	if (pItemIDList)
	{
		SHGetPathFromIDList(pItemIDList, szPath);
		IMalloc *pMalloc;
		if (SHGetMalloc(&pMalloc) != NOERROR)
		{
			AfxMessageBox(LANG_CS("path choose failed!"));
			return;
		}
		
		int pos  = m_csSrcFilePath.ReverseFind('\\');
		CString FileName ="\\";
		FileName += m_csSrcFilePath.Right(m_csSrcFilePath.GetLength() - pos - 1);
		FileName.Delete(FileName.GetLength() - 3, 3);
		FileName +="pcm";
		
		m_csAudiofilePath = szPath  + FileName;
		SetDlgItemText(IDC_EDIT_AUDIOFILEPATH, m_csAudiofilePath);
		pMalloc->Free(pItemIDList);
		if(pMalloc)
			pMalloc->Release();
	}
}

void CDecCB_demoDlg::OnButtonVideofile() 
{
	// TODO: Add your control notification handler code here
	CString szTitle = LANG_CS("folder");
	TCHAR szDisplayName[MAX_PATH] =_T("");
	TCHAR szPath[MAX_PATH] =_T("");
	BROWSEINFO bi;
	bi.hwndOwner = GetSafeHwnd();
	bi.pidlRoot = NULL;
	bi.lpszTitle = szTitle;
	bi.pszDisplayName = szDisplayName;
	bi.ulFlags = BIF_RETURNONLYFSDIRS|BIF_BROWSEINCLUDEFILES;
	bi.lpfn = NULL;
	bi.lParam = 0;
	LPITEMIDLIST pItemIDList = SHBrowseForFolder(&bi);
	
	if (pItemIDList)
	{
		SHGetPathFromIDList(pItemIDList, szPath);
		IMalloc *pMalloc;
		if (SHGetMalloc(&pMalloc) != NOERROR)
		{
			AfxMessageBox(LANG_CS("path choose failed!"));
			return;
		}

		int pos  = m_csSrcFilePath.ReverseFind('\\');
		CString FileName ="\\";
		FileName += m_csSrcFilePath.Right(m_csSrcFilePath.GetLength() - pos - 1);
		FileName.Delete(FileName.GetLength() - 3, 3);
		FileName +="yuv";

		m_csVideofilePath = szPath  + FileName;

		SetDlgItemText(IDC_EDIT_VIDEOFILEPATH, m_csVideofilePath);
		pMalloc->Free(pItemIDList);
		if(pMalloc)
			pMalloc->Release();
	}
}

void CDecCB_demoDlg::OnInterfaceNew() 
{
	// TODO: Add your control notification handler code here
	m_nInterfaceType = 0;
}

void CDecCB_demoDlg::OnInterfaceOld() 
{
	// TODO: Add your control notification handler code here
	m_nInterfaceType = 1;
}

void CDecCB_demoDlg::OnInterfaceNew2() 
{
	// TODO: Add your control notification handler code here
	m_nInterfaceType = 2;
}

void CDecCB_demoDlg::OnInterfaceOld2() 
{
	// TODO: Add your control notification handler code here
	m_nInterfaceType = 3;
}

BOOL CDecCB_demoDlg::PreTranslateMessage(MSG* pMsg) 
{
	// TODO: Add your specialized code here and/or call the base class
	if(pMsg->message == WM_KEYDOWN && (pMsg->wParam == VK_RETURN || pMsg->wParam == VK_ESCAPE))
		return TRUE;
	return CDialog::PreTranslateMessage(pMsg);
}
