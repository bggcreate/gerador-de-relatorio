#pragma once


// DecodeEngineDlg 对话框

class DecodeEngineDlg : public CDialog
{
	DECLARE_DYNAMIC(DecodeEngineDlg)

public:
	DecodeEngineDlg(CWnd* pParent = NULL);   // 标准构造函数
	virtual ~DecodeEngineDlg();

// 对话框数据
	enum { IDD = IDD_DIALOG_DECODER_ENGINE };

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV 支持
	virtual BOOL OnInitDialog();

private:
	int m_nType;
public:
	int GetDecodeType(){return m_nType;}
public:
	DECLARE_MESSAGE_MAP()
	afx_msg void OnBnClickedOk();
};
