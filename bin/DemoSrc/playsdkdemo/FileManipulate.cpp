// FileManipulate.cpp: implementation of the FileManipulate class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "FileManipulate.h"

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

FileManipulate::FileManipulate()
{
	m_hFile = NULL;
	m_pFile = NULL;
}

FileManipulate::~FileManipulate()
{

}

int64 FileManipulate::getFileTotalSize()
{
	int64 nFileTotal = 0;
#ifdef LINUX_ENABLE
	if(m_pFile == NULL)
		return 0;

	long long nOffset = 0;
	fgetpos(m_pFile, &nOffset);
	fseek(m_pFile, 0, SEEK_END);
	fgetpos(m_pFile, &nFileTotal);
	fsetpos(m_pFile, &nOffset);

  return nFileTotal;

#endif
  
#ifdef WIN32
  if(m_hFile == INVALID_HANDLE_VALUE)
	  return 0;

	DWORD dwfileSizeHigh = 0;
	DWORD nFileSizeLow = GetFileSize(m_hFile, &dwfileSizeHigh);
	
	LARGE_INTEGER largeLen;
	largeLen.HighPart = dwfileSizeHigh;
	largeLen.LowPart = nFileSizeLow;
	nFileTotal = largeLen.QuadPart;

	return nFileTotal;
#endif
}

bool FileManipulate::getPos(int64 &nCurrentPos)
{
#ifdef	LINUX_ENABLE
	if(m_pFile == NULL)
		return false;
	if (!fgetpos(m_pFile, nCurrentPos))
		return true;
	else
		return false;
#endif

#ifdef WIN32
	if(m_hFile == INVALID_HANDLE_VALUE)
		return false;
	else
	{
		LARGE_INTEGER li;
		li.HighPart = 0;
		li.LowPart = SetFilePointer(m_hFile, 0, &li.HighPart, FILE_CURRENT);
		nCurrentPos = li.QuadPart;
		return true;
	}
		
#endif
}

bool FileManipulate::setPos(int64 pos, unsigned long dwMoveMethod)
{
#ifdef LINUX_ENABLE
	if(m_pFile == NULL)
		return FALSE;
	if(!fsetpos(m_pFile, &pos))
		return TRUE;//返回0为成功
	else
		return FALSE;
#endif
	
#ifdef WIN32
	if(m_hFile == INVALID_HANDLE_VALUE)
		return false;
	LARGE_INTEGER li;
	li.QuadPart = pos;

	if(SetFilePointer(m_hFile, li.LowPart, &li.HighPart, dwMoveMethod) == -1)
		return FALSE;//返回-1为失败
	else
		return TRUE;
#endif
}

bool FileManipulate::openFile(char *strName, int nMode)
{
#ifdef LINUX_ENABLE
	if(nMode == READ_ONLY)
		m_pFile = fopen(strName, "rb");
	else if(nMode == WRITE_ONLY)
		m_pFile = fopen(strName, "wb");
	else if(nMode == WRITE_SUPER)
		fopen(strName, "a");
	else if(nMode == READ_WRITE)
		fopen(strName, "r+");
	else if(nMode == READ_WRITE_SUPER)
		fopen(strName, "ab+");

	if(m_pFile == NULL)
		return FALSE;
	else
		return TRUE;
#endif
	  
#ifdef WIN32
	if(nMode == READ_ONLY)
		m_hFile = CreateFile(strName, 
							GENERIC_READ, 
							FILE_SHARE_READ, 
							NULL, 
							OPEN_EXISTING, 
							FILE_ATTRIBUTE_NORMAL, 
							NULL);
	else if(nMode == WRITE_ONLY)
		m_hFile = CreateFile(strName, 
							GENERIC_WRITE, 
							FILE_SHARE_WRITE, 
							NULL, 
							OPEN_EXISTING, 
							FILE_ATTRIBUTE_NORMAL, 
							NULL);
	else if(nMode == WRITE_SUPER)
		m_hFile = CreateFile(strName, 
							GENERIC_WRITE, 
							FILE_SHARE_WRITE, 
							NULL, 
							OPEN_ALWAYS, 
							FILE_ATTRIBUTE_NORMAL, 
							NULL);
	else if(nMode == READ_WRITE)
		m_hFile = CreateFile(strName, 
							GENERIC_ALL, 
							FILE_SHARE_WRITE|FILE_SHARE_READ, 
							NULL, 
							OPEN_EXISTING, 
							FILE_ATTRIBUTE_NORMAL, 
							NULL);
	else if(nMode == READ_WRITE_SUPER)
		m_hFile = CreateFile(strName, 
							GENERIC_ALL, 
							FILE_SHARE_WRITE|FILE_SHARE_READ, 
							NULL, 
							OPEN_ALWAYS, 
							FILE_ATTRIBUTE_NORMAL, 
							NULL);
	if(m_hFile == INVALID_HANDLE_VALUE)
		return FALSE;
	else
		return TRUE;
#endif
}

bool FileManipulate::readFile(void *lpBuffer, unsigned long nNumberOfBytesToRead, unsigned long *lpNumberOfBytesRead)
{
#ifdef LINUX_ENABLE
	*lpNumberOfBytesRead = fread(lpBuffer,1, nNumberOfBytesToRead, m_pFile);
	if(*lpNumberOfBytesRead <= 0)
		return FALSE;
	else
		return TRUE;
#endif
  
#ifdef WIN32
	if(m_hFile == INVALID_HANDLE_VALUE)
		return FALSE;
	return ReadFile(m_hFile, lpBuffer, nNumberOfBytesToRead, lpNumberOfBytesRead, NULL);
#endif
}

bool FileManipulate::writeFile(const char *lpBuffer, unsigned long nNumberOfBytesToWrite, unsigned long *lpNumberOfBytesWritten)
{
#ifdef LINUX_ENABLE
	*lpNumberOfBytesWritten = fwrite(lpBuffer, 1, nNumberOfBytesToWrite, m_pFile);
	if(*lpNumberOfBytesWritten <= 0)
		return FALSE;
	else
		return TRUE;
#endif
	  
#ifdef WIN32
	if(m_hFile == INVALID_HANDLE_VALUE)
		return FALSE;
	return WriteFile(m_hFile, lpBuffer, nNumberOfBytesToWrite, lpNumberOfBytesWritten, NULL);
#endif
}

bool FileManipulate::closeFile()
{	
#ifdef LINUX_ENABLE
	if(m_pFile == NULL)
		return FALSE;

	if(fclose(m_pFile) == 0)
		return TRUE;
	else
		return FALSE;
#endif
	  
#ifdef WIN32
	if(m_hFile == INVALID_HANDLE_VALUE)
		return FALSE;
	return CloseHandle(m_hFile);
#endif
}
