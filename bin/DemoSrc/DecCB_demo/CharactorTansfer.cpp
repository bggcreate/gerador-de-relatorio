// stdafx.cpp : source file that includes just the standard includes
//	PlayDemo.pch will be the pre-compiled header
//	stdafx.obj will contain the pre-compiled type information

#include "stdafx.h"

#include "CharactorTansfer.h"
//gbk转UTF-8
std::string GbkToUtf8(const std::string& strGbk)//传入的strGbk是GBK编码
{
	//gbk转unicode
	int len = MultiByteToWideChar(CHINESE_CODE_PAGE, 0, strGbk.c_str(), -1, NULL, 0);
	wchar_t *strUnicode = new wchar_t[len];
	wmemset(strUnicode, 0, len);
	MultiByteToWideChar(CHINESE_CODE_PAGE, 0, strGbk.c_str(), -1, strUnicode, len);

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

//UTF-8转gbk
std::string Utf8ToGbk(const std::string& strUtf8)//传入的strUtf8是UTF-8编码
{
	//UTF-8转unicode
	int len = MultiByteToWideChar(CP_UTF8, 0, strUtf8.c_str(), -1, NULL, 0);
	wchar_t * strUnicode = new wchar_t[len];//len = 2
	wmemset(strUnicode, 0, len);
	MultiByteToWideChar(CP_UTF8, 0, strUtf8.c_str(), -1, strUnicode, len);

	//unicode转gbk
	len = WideCharToMultiByte(CHINESE_CODE_PAGE, 0, strUnicode, -1, NULL, 0, NULL, NULL);
	char *strGbk = new char[len];//len=3 本来为2，但是char*后面自动加上了\0
	memset(strGbk, 0, len);
	WideCharToMultiByte(CHINESE_CODE_PAGE,0, strUnicode, -1, strGbk, len, NULL, NULL);

	std::string strTemp(strGbk);//此时的strTemp是GBK编码
	delete[]strUnicode;
	delete[]strGbk;
	strUnicode = NULL;
	strGbk = NULL;
	return strTemp;
}

//gbk转unicode (下面的例子没用到)
std::wstring GbkToUnicode(const std::string& strGbk)//返回值是wstring
{
	int len = MultiByteToWideChar(CHINESE_CODE_PAGE, 0, strGbk.c_str(), -1, NULL, 0);
	wchar_t *strUnicode = new wchar_t[len];
	wmemset(strUnicode, 0, len);
	MultiByteToWideChar(CHINESE_CODE_PAGE, 0, strGbk.c_str(), -1, strUnicode, len);

	std::wstring strTemp(strUnicode);//此时的strTemp是Unicode编码
	delete[]strUnicode;
	strUnicode = NULL;
	return strTemp;
}

//Unicode转gbk
std::string UnicodeToGbk (const std::wstring& strUnicode)//参数是wstring
{
	int len = WideCharToMultiByte(CHINESE_CODE_PAGE, 0, strUnicode.c_str(), -1, NULL, 0, NULL, NULL);
	char *strGbk = new char[len];//len=3 本来为2，但是char*后面自动加上了\0
	memset(strGbk, 0, len);
	WideCharToMultiByte(CHINESE_CODE_PAGE,0,strUnicode.c_str(), -1, strGbk, len, NULL, NULL);
	
	std::string strTemp(strGbk);
	delete []strGbk;
	strGbk = NULL;
	return strTemp;
}
