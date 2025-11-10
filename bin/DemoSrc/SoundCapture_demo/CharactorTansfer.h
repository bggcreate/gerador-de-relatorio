#ifndef CHARACTOR_TANSFER
#define CHARACTOR_TANSFER

#include <string>

#define CHINESE_CODE_PAGE    936

std::string UnicodeToGbk (const std::wstring& strUnicode);//参数是wstring
std::wstring GbkToUnicode(const std::string& strGbk);
#endif