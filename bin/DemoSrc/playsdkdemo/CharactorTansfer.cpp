// stdafx.cpp : source file that includes just the standard includes
//	PlayDemo.pch will be the pre-compiled header
//	stdafx.obj will contain the pre-compiled type information

#include "stdafx.h"

#include "CharactorTansfer.h"
//Ansi转UTF-8
std::string AnsiToUtf8(const std::string& strAnsi)//传入的strAnsi是Ansi编码
{
	//Ansi转unicode
	int len = MultiByteToWideChar(CP_ACP, 0, strAnsi.c_str(), -1, NULL, 0);
	wchar_t *strUnicode = new wchar_t[len];
	wmemset(strUnicode, 0, len);
	MultiByteToWideChar(CP_ACP, 0, strAnsi.c_str(), -1, strUnicode, len);

	//unicode转UTF-8
	len = WideCharToMultiByte(CP_UTF8, 0, strUnicode, -1, NULL, 0, NULL, NULL);
	char * strUtf8 = new char[len];
	WideCharToMultiByte(CP_UTF8, 0, strUnicode, -1, strUtf8, len, NULL, NULL);

	std::string strTemp(strUtf8);//此时的strTemp是UTF-8编码
	delete[]strUnicode;
	delete[]strUtf8;
	strUnicode = NULL;
	strUtf8 = NULL;
	return strTemp;
}

//UTF-8转Ansi
std::string Utf8ToAnsi(const std::string& strUtf8)//传入的strUtf8是UTF-8编码
{
	//UTF-8转unicode
	int len = MultiByteToWideChar(CP_UTF8, 0, strUtf8.c_str(), -1, NULL, 0);
	wchar_t * strUnicode = new wchar_t[len];//len = 2
	wmemset(strUnicode, 0, len);
	MultiByteToWideChar(CP_UTF8, 0, strUtf8.c_str(), -1, strUnicode, len);

	//unicode转Ansi
	len = WideCharToMultiByte(CP_ACP, 0, strUnicode, -1, NULL, 0, NULL, NULL);
	char *strAnsi = new char[len];//len=3 本来为2，但是char*后面自动加上了\0
	memset(strAnsi, 0, len);
	WideCharToMultiByte(CP_ACP,0, strUnicode, -1, strAnsi, len, NULL, NULL);

	std::string strTemp(strAnsi);//此时的strTemp是Ansi编码
	delete[]strUnicode;
	delete[]strAnsi;
	strUnicode = NULL;
	strAnsi = NULL;
	return strTemp;
}

//Ansi转unicode (下面的例子没用到)
std::wstring AnsiToUnicode(const std::string& strAnsi)//返回值是wstring
{
	int len = MultiByteToWideChar(CP_ACP, 0, strAnsi.c_str(), -1, NULL, 0);
	wchar_t *strUnicode = new wchar_t[len];
	wmemset(strUnicode, 0, len);
	MultiByteToWideChar(CP_ACP, 0, strAnsi.c_str(), -1, strUnicode, len);

	std::wstring strTemp(strUnicode);//此时的strTemp是Unicode编码
	delete[]strUnicode;
	strUnicode = NULL;
	return strTemp;
}

//Unicode转Ansi
std::string UnicodeToAnsi (const std::wstring& strUnicode)//参数是wstring
{
	int len = WideCharToMultiByte(CP_ACP, 0, strUnicode.c_str(), -1, NULL, 0, NULL, NULL);
	char *strAnsi = new char[len];//len=3 本来为2，但是char*后面自动加上了\0
	memset(strAnsi, 0, len);
	WideCharToMultiByte(CP_ACP,0,strUnicode.c_str(), -1, strAnsi, len, NULL, NULL);
	
	std::string strTemp(strAnsi);
	delete []strAnsi;
	strAnsi = NULL;
	return strTemp;
}
