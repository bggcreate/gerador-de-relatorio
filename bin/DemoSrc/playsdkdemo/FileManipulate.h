// FileManipulate.h: interface for the FileManipulate class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_FILEMANIPULATE_H__307F377B_8EB8_4327_9702_484DD19A7A06__INCLUDED_)
#define AFX_FILEMANIPULATE_H__307F377B_8EB8_4327_9702_484DD19A7A06__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
#include <STDIO.H>

#if (defined(WIN32) || defined(WIN64))
typedef __int64	int64;
#else
typedef long long int64;
#endif
enum
{
	READ_ONLY,
		WRITE_ONLY,
		READ_WRITE,
		READ_WRITE_SUPER,
		WRITE_SUPER
};
enum
{
	GETPOS,
	SETPOS
};
class FileManipulate  
{
public:
	
	FileManipulate();
	virtual ~FileManipulate();
public:
	int64 getFileTotalSize();
	bool setPos(int64 pos, unsigned long dwMoveMethod);
	bool getPos(int64 &nCurrentPos);
	bool openFile(char *strName, int nMode);
	bool writeFile(const char *lpBuffer, unsigned long nNumberOfBytesToWrite, unsigned long *lpNumberOfBytesWritten);
	bool readFile(void *lpBuffer, unsigned long nNumberOfBytesToRead, unsigned long *lpNumberOfBytesRead);
	bool closeFile();
private:
	HANDLE m_hFile;
	FILE *m_pFile;

};

#endif // !defined(AFX_FILEMANIPULATE_H__307F377B_8EB8_4327_9702_484DD19A7A06__INCLUDED_)
