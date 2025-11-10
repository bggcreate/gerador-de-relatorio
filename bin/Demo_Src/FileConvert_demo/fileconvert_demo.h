
#ifndef FileConvert_demo_H
#define FileConvert_demo_H

#include <QDialog>
#include "fileconvertthread.h"
#include "dhplay.h"

typedef enum
{
    AVI_CONVERT,
    ASF_CONVERT,
    MP4_CONVERT,
    PS_CONVERT,
    TS_CONVERT,
}ConvertType;

namespace Ui {
class FileConvert_demo;
}

class FileConvert_demo : public QDialog, public IRunnable
{
    Q_OBJECT
    
public:
    explicit FileConvert_demo(QWidget *parent = 0);
    ~FileConvert_demo();

private:
    enum UI_STATE
    {
        CONVERT,
        CANCEL
    };

private slots:
    void on_StartConvert_clicked();

    void on_StopConvert_clicked();

    void on_BrowseSourcePath_clicked();

    void on_BrowseDestPath_clicked();

    void onStartConvertFailed();
    void onOpenFileFailed();
    void onConvertCompleted();
    void onValueChanged(int value);
    void onChangeUIState(int state);

    void on_radioBtnAVI_clicked();
    void on_radioBtnASF_clicked();
    void on_radioBtnMP4_clicked();
    void on_radioBtnTS_clicked();
    void on_radioBtnPS_clicked();

signals:
    void startConvertFailed();
    void openFileFailed();
    void convertCompleted();
    void valueChanged(int value);
    void changeUIState(int state);

public:
    virtual void run();

public:
	QString m_csSourceFile;
	QString m_csTargetFile;
    void onAviCBFunc(LONG nPort, LONG lMediaChangeType, BOOL *pbIfContinue, char *sNewFileName);
    void closeEvent(QCloseEvent *event);
    int convertType(int nDataType);
private:
    void changeExtensionName(const QString &srcExtName, const QString &dstExtName);
    bool startConvert();
    void stopConvert();

private:
    Ui::FileConvert_demo *ui;
    FileConvertThread    *m_convertThread;
    bool                 m_isConverting;
    int                  m_nChangeCount;
    int                  m_nConvertType;

};

#endif // FileConvert_demo_H
