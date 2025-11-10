#pragma once


#ifdef _DEBUG
// CCmdDialog 对话框

class CCmdDialog : public CDialog
{
	DECLARE_DYNAMIC(CCmdDialog)

public:
	CCmdDialog(CWnd* pParent = NULL);   // 标准构造函数
	virtual ~CCmdDialog();

// 对话框数据
	enum { IDD = IDD_DIALOG_CONSOLE };

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV 支持

	DECLARE_MESSAGE_MAP()

	virtual void OnOK();

private:
	void HandleMessage();
	CString	m_strCommand;
};

#endif