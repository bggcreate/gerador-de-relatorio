// CmdDialog.cpp : 实现文件
//

#include "stdafx.h"
#include "PlayDemo.h"
#include "CmdDialog.h"
#include "CharactorTansfer.h"

#include <assert.h>
#include <string>

// CCmdDialog 对话框
#ifdef _DEBUG

IMPLEMENT_DYNAMIC(CCmdDialog, CDialog)

CCmdDialog::CCmdDialog(CWnd* pParent /*=NULL*/)
	: CDialog(CCmdDialog::IDD, pParent)
{
	m_strCommand = _T("");
}

CCmdDialog::~CCmdDialog()
{
}

void CCmdDialog::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	DDX_Text(pDX, IDC_EDIT_COMMAND, m_strCommand);
}


BEGIN_MESSAGE_MAP(CCmdDialog, CDialog)
END_MESSAGE_MAP()

void CCmdDialog::OnOK()
{
	CDialog::OnOK();
	HandleMessage();
}

template<typename TRET>
class TProxy
{
public:
	static TRET Invoke(const char* moduleName, const char* methodName)
	{
		typedef TRET (WINAPI *MethodPtr)();
		MethodPtr func = NULL;
		func = (MethodPtr)::GetProcAddress(module,methodName);
		return (func == NULL) ? TRET() : func();
	}

	template<typename T1>
	static TRET Invoke(const char* moduleName, const char* methodName, T1 t1)
	{
		typedef TRET (WINAPI *MethodPtr)(T1);
		MethodPtr func = NULL;
		func = (MethodPtr)::GetProcAddress(module,methodName);
		return (func == NULL) ? TRET() : func(t1);
	}

	template<typename T1, typename T2>
	static TRET Invoke(const char* moduleName, const char* methodName, T1 t1, T2 t2)
	{
		typedef TRET (WINAPI *MethodPtr)(T1, T2);
		MethodPtr func = NULL;
		func = (MethodPtr)::GetProcAddress(module,methodName);
		return (func == NULL) ? TRET() : func(t1, t2);
	}

	template<typename T1, typename T2, typename T3>
	static TRET Invoke(const char* moduleName, const char* methodName, T1 t1, T2 t2, T3 t3)
	{
		typedef TRET (WINAPI *MethodPtr)(T1, T2, T3);
		MethodPtr func = NULL;
		func = (MethodPtr)::GetProcAddress(module,methodName);
		return (func == NULL) ? TRET() : func(t1, t2, t3);
	}
private:
	static HMODULE module;
};

template<typename TRET>
HMODULE TProxy<TRET>::module = GetModuleHandle(_T("dhplay.dll"));

void CCmdDialog::HandleMessage()
{
	CString cmd = m_strCommand;
	int left,right;
	right = cmd.Find('(');
	CString funcName = cmd.Left(right);

	CString p[4];
	int in_p[4];
	CHAR t[MAX_PATH] = {0};
	int stringNum = 0;
	CString str;
	std::string strParam;
	int pn = 0;
	while(pn < 4)
	{
		left = right + 1;
		right = cmd.Find(',',left);
		if(right == -1)
			break;
		p[pn++] = cmd.Mid(left,right - left);
	}

	right = cmd.Find(')',left);
	p[pn++] = cmd.Mid(left,right - left);

	for(int i = 0;i < pn;i++)
	{
		left = p[i].Find('"');
		// string
		if(left != -1)
		{
			assert(stringNum == 0);
			right = p[i].Find('"',left + 1);
			str = p[i].Mid(left + 1,right - left - 1);
			strParam = UnicodeToAnsi(str.GetBuffer(str.GetLength()));
			strncpy(t, strParam.c_str(), MAX_PATH);
			in_p[i] = (int)t;
			continue;
		}

		left = p[i].Find('.');
		// float
		if(left != -1)
		{
			float f = _tstof(p[i]);
			memcpy(&in_p[i],&f,sizeof(f));
			continue;
		}

		left = p[i].Find('x');
		// float
		if(left != -1)
		{
			in_p[i] = _tcstol(p[i],NULL,16);
			continue;
		}

		//int
		in_p[i] = _ttoi(p[i]);
	}
	std::string strFuncName = UnicodeToAnsi(funcName.GetBuffer(funcName.GetLength()));
	BOOL ret = 0;
	switch(pn)
	{
	case 0:
		ret = TProxy<BOOL>::Invoke("dhplay.dll",strFuncName.c_str());
		break;
	case 1:
		ret = TProxy<BOOL>::Invoke("dhplay.dll",strFuncName.c_str(),in_p[0]);
		break;
	case 2:
		ret = TProxy<BOOL>::Invoke("dhplay.dll",strFuncName.c_str(),in_p[0],in_p[1]);
		break;
	case 3:
		ret = TProxy<BOOL>::Invoke("dhplay.dll",strFuncName.c_str(),in_p[0],in_p[1],in_p[2]);
		break;
	}

	if(!ret)
	{
		char text[MAX_PATH * 2];
		sprintf(text,"%s Failed!",cmd);
		std::wstring wstrText = AnsiToUnicode(text);

		MessageBox(wstrText.c_str(),_T("ERROR"),0);
	}

}
// CCmdDialog 消息处理程序
#endif