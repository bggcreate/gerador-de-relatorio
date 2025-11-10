// LanguageConvertor.cpp: implementation of the CLanguageConvertor class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "PlayDemo.h"
#include "LanguageConvertor.h"

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////
TCHAR CLanguageConvertor::m_szIniFile[MAX_PATH] = {0};


int CLanguageConvertor::Init()
{
	_stprintf(m_szIniFile, _T("%s%s"), GetMoudlePath(), _T("lang.ini"));
	return 1;
}

CString CLanguageConvertor::ConvertString(CString strText)
{
	if(strText.GetLength())
	{
		/*find the value of the variable whose keyword is strText in the translation file*/
		TCHAR val[300];
		GetPrivateProfileString(_T("String"), strText, strText, val, sizeof(val), m_szIniFile);
		return val;
	}

	return strText;
}

void CLanguageConvertor::SetWndStaticText(CWnd * pWnd)
{
	CString strCaption,strText;

	pWnd->GetWindowText(strCaption);
	if(strCaption.GetLength()>0)
	{
		strText=ConvertString(strCaption);
		pWnd->SetWindowText(strText);
	}

	CWnd * pChild=pWnd->GetWindow(GW_CHILD);
	CString strClassName;
	while(pChild)
	{
		strClassName = ((CRuntimeClass*)pChild->GetRuntimeClass())->m_lpszClassName;
		if(strClassName == "CEdit")
		{
			pChild=pChild->GetWindow(GW_HWNDNEXT);
			continue;
		}
		//////////////////////////////////////////////////////////////////////////
		pChild->GetWindowText(strCaption);
		strText=ConvertString(strCaption);
		pChild->SetWindowText(strText);
		pChild=pChild->GetWindow(GW_HWNDNEXT);
	}
}

void CLanguageConvertor::SetMenuStaticText(CMenu* pMenu)
{
	CString strCaption,strText;
	
	int MenuItemNum = pMenu->GetMenuItemCount() ;
	
	for (int i = 0 ; i < MenuItemNum ; i++)
	{		
		MENUITEMINFO info;
		memset(&info, 0 , sizeof(MENUITEMINFO)) ;
		info.cbSize = sizeof (MENUITEMINFO); // must fill up this field
		info.fMask = MIIM_STATE;             // get the state of the menu item
		
		pMenu->GetMenuString(i, strCaption, MF_BYPOSITION);
		
		strText=ConvertString(strCaption);
		
		UINT ID = pMenu->GetMenuItemID(i) ;
		
		if (ID != -1)
		{
			pMenu->GetMenuItemInfo(ID, &info) ;
			pMenu->ModifyMenu(ID, MF_BYCOMMAND|MF_STRING, ID, strText);
			SetMenuItemInfo(pMenu->m_hMenu, ID, FALSE, &info) ;
		}
		else
		{
			pMenu->ModifyMenu(i, MF_BYPOSITION| MF_STRING, 0, strText);
			if (strCaption == "Fisheye View")
			{
				pMenu->EnableMenuItem(i, MF_BYPOSITION| MF_GRAYED);
			}
		}
		
		CMenu* subMenu = pMenu->GetSubMenu(i) ;
		
		if (subMenu != NULL)
		{			
			SetMenuStaticText(subMenu) ;
		}
	}
}