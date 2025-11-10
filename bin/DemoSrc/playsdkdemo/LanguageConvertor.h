// LanguageConvertor.h: interface for the CLanguageConvertor class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_LANGUAGECONVERTOR_H__DCA4B0F1_D7A4_4376_940F_070756A2B2DA__INCLUDED_)
#define AFX_LANGUAGECONVERTOR_H__DCA4B0F1_D7A4_4376_940F_070756A2B2DA__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "StdAfx.h"

#define LANG_INIT() CLanguageConvertor::Init()
#define LANG_CS(a) CLanguageConvertor::ConvertString(a)
#define LANG_SETWNDSTATICTEXT(a) CLanguageConvertor::SetWndStaticText(a)
#define LANG_SETMENUSTATICTEXT(a) CLanguageConvertor::SetMenuStaticText(a)

class CLanguageConvertor  
{
public:
	static int Init();

	/*String Language convert */
	static CString ConvertString(CString strText);                 

	/* static Text on Wnd Language convert  */
	static void SetWndStaticText(CWnd * pWnd);   
	
	/* static Text on menu  Language Convert */
	static void SetMenuStaticText(CMenu* pMenu);

private:
	static TCHAR m_szIniFile[MAX_PATH];	
};

#endif // !defined(AFX_LANGUAGECONVERTOR_H__DCA4B0F1_D7A4_4376_940F_070756A2B2DA__INCLUDED_)
