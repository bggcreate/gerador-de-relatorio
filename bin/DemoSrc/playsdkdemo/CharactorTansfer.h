#ifndef CHARACTOR_TANSFER
#define CHARACTOR_TANSFER

#include <string>

std::string UnicodeToAnsi (const std::wstring& strUnicode);//²ÎÊýÊÇwstring
std::wstring AnsiToUnicode(const std::string& strAnsi);
#endif