#ifndef __FISHEYE_PTZ_HANDLER_H_
#define __FISHEYE_PTZ_HANDLER_H_

class CFishEyePTZSetImpl
{
public:	
	virtual bool StartDrag(CPoint point, HWND hwin) = 0;
	virtual void StopDrag() = 0;
	virtual void UpdateDrag(CPoint point) = 0;
	virtual void SetPointInfo(CPoint point, int index) = 0;
	virtual void SetArg(int arg4, int arg5, int index) = 0;
	virtual void CommitSet() = 0;

};

class CFishEyePTZSet : public CFishEyePTZSetImpl
{
public:
	CFishEyePTZSet();

public:
	virtual bool StartDrag(CPoint point, HWND hwin);
	virtual void StopDrag();
	virtual void UpdateDrag(CPoint point);
	virtual void SetPointInfo(CPoint point, int index);
	virtual void SetArg(int arg4, int arg5, int index);
	virtual void CommitSet();

protected:
	int m_feCurWin;
	CPoint m_feMovePoint;
	HWND m_win;

private:
	CPoint m_feLastPoint;
};

class CFishEyePTZSet_0 : public CFishEyePTZSet
{
public:
	CFishEyePTZSet_0();

public:
	bool StartDrag(CPoint point, HWND hwin);
	void StopDrag();
	void UpdateDrag(CPoint point);
	void SetPointInfo(CPoint point, int index);
	void SetArg(int arg4, int arg5, int index);
	void CommitSet();

private:
	int isLeft(CPoint P0, CPoint P1, CPoint P2);
	bool PtInPolygon(const CPoint pt, CPoint* polygon, const int size);

private:
	CPoint m_fePointInfo[480];
	int m_arg4[3];
	int m_arg5[3];
};

class CDrawer
{
public:
	CDrawer(HWND hwin, HDC hdc);
	void DrawLine(POINT p1, POINT p2, int red, int green, int blue);
	void DrawRect(POINT p1, POINT p2, int red, int green, int blue);
	void GetWidthAndHeight(int& width, int& height);
	
private:
	HWND _hwin;
	HDC _hdc;
	
	int _width;
	int _height;
};

class CFishEyePTZHandler
{
public:
	CFishEyePTZHandler();

public:
	void Draw(HWND hwin, HDC hdc);
	bool StartDrag(CPoint point);
	void StopDrag();
	void UpdateDrag(CPoint point);

private:
	bool isOWin(CPoint point);
	void DrawFishEyeOneWin(int winid, int red, int green, int blue);

private:
	CFishEyePTZSetImpl* m_impl;
	CFishEyePTZSet m_FEPimpl;
	CFishEyePTZSet_0 m_FEPimpl_0;

	CRITICAL_SECTION	m_cs;
	HWND m_win;
	HDC m_dc;
};

#endif // __FISHEYE_PTZ_HANDLER_H_
