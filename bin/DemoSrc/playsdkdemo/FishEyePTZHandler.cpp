#include "StdAfx.h"
#include "Player.h"

CFishEyePTZSet::CFishEyePTZSet()
{
	m_feCurWin = 0;
	m_feMovePoint.x = m_feMovePoint.y = 0;
	m_win = 0;
	
	m_feLastPoint.x = m_feLastPoint.y = 0;
}

bool CFishEyePTZSet::StartDrag(CPoint point, HWND hwin)
{
	m_win = hwin;

	CDrawer d(m_win, 0);
	int w,h;
	d.GetWidthAndHeight(w, h);

	if(point.x > w/2 && point.y < h/2)
	{
		m_feCurWin = 1;
	}

	if(point.x < w/2 && point.y > h/2)
	{
		m_feCurWin = 2;
	}

	if(point.x > w/2 && point.y > h/2)
	{
		m_feCurWin = 3;
	}

	return true;
}

void CFishEyePTZSet::StopDrag()
{
	m_feMovePoint.x = 0;
	m_feMovePoint.y = 0;
	CommitSet();
	m_feCurWin = -1;
}

void CFishEyePTZSet::UpdateDrag(CPoint point)
{
	if(m_feCurWin != -1)
	{
		m_feMovePoint = point;
	}
	else
	{
		m_feMovePoint.x = 0;
		m_feMovePoint.y = 0;
	}
}

void CFishEyePTZSet::SetPointInfo(CPoint point, int index)
{
	
}

void CFishEyePTZSet::SetArg(int arg4, int arg5, int index)
{
	
}

void CFishEyePTZSet::CommitSet()
{
	if(m_feCurWin != -1)
	{
		if(m_feMovePoint.x == 0 || m_feMovePoint.y == 0 || m_feLastPoint.x == 0 || m_feLastPoint.y == 0)
		{
			m_feLastPoint = m_feMovePoint;
		}

		CDrawer d(m_win, 0);
		int w, h;
		d.GetWidthAndHeight(w, h);
		
		FISHEYE_EPTZPARAM eptzParams;	
		memset(&eptzParams, 0, sizeof(eptzParams));
		eptzParams.winId = m_feCurWin;
		eptzParams.ePtzCmd = FISHEYEEPTZ_CMD_SET_CUR_REGION;
		eptzParams.arg1 = 18;
		
		eptzParams.arg2 = (m_feMovePoint.x - m_feLastPoint.x)* 8191 / (w/2);
		eptzParams.arg3 = (m_feMovePoint.y - m_feLastPoint.y)* 8191 / (h/2);
		
		m_feLastPoint = m_feLastPoint;

		CPlayer::Instance()->FisheyeEptzUpdate(&eptzParams, 0);
	}
}

CFishEyePTZSet_0::CFishEyePTZSet_0()
: CFishEyePTZSet()
{
}

bool CFishEyePTZSet_0::StartDrag(CPoint point, HWND hwin)
{
	m_win = hwin;

	int i;
	for(i = 0; i < 3; i++)
	{
		bool ret = PtInPolygon(point, m_fePointInfo + i * 160, 160);
		if(ret)
		{
			break;
		}
	}
	
	if(i != 3)
	{
		m_feCurWin = ++i;
		m_feMovePoint = point;
	}

	return true;
}

void CFishEyePTZSet_0::StopDrag()
{
	m_feCurWin = -1;
}

void CFishEyePTZSet_0::UpdateDrag(CPoint point)
{
	if(m_feCurWin != -1)
	{
		m_feMovePoint = point;
	}
	else
	{
		m_feMovePoint.x = 0;
		m_feMovePoint.y = 0;
	}
}

void CFishEyePTZSet_0::SetPointInfo(CPoint point, int index)
{
	m_fePointInfo[index] = point;
}

void CFishEyePTZSet_0::SetArg(int arg4, int arg5, int index)
{
	m_arg4[index] = arg4;
	m_arg5[index] = arg5;
}

void CFishEyePTZSet_0::CommitSet()
{
	if(m_feCurWin != -1)
	{
		CDrawer d(m_win, 0);
		int w, h;
		d.GetWidthAndHeight(w, h);

		FISHEYE_EPTZPARAM eptzParams;	
		memset(&eptzParams, 0, sizeof(eptzParams));
		eptzParams.winId = m_feCurWin;
		eptzParams.ePtzCmd = FISHEYEEPTZ_CMD_SET_CUR_REGION;
		eptzParams.arg1 = 0;
		
		eptzParams.arg2 = m_feMovePoint.x * 8191 / (w/2);
		eptzParams.arg3 = m_feMovePoint.y * 8191 / (h/2);
		eptzParams.arg4 = m_arg4[m_feCurWin-1];
		eptzParams.arg5 = m_arg5[m_feCurWin-1];
		
		CPlayer::Instance()->FisheyeEptzUpdate(&eptzParams, 0);
	}
}

int CFishEyePTZSet_0::isLeft(CPoint P0, CPoint P1, CPoint P2)
{
	return ( (P1.x - P0.x) * (P2.y - P0.y) - (P2.x - P0.x) * (P1.y - P0.y) );
}

bool CFishEyePTZSet_0::PtInPolygon(const CPoint pt, CPoint* polygon, const int size)
{
	int wn = 0;
	for (int i = 0 ; i < size - 1; i++)
	{
		if (i >= 1 )
		{
			if (polygon[i].x == 0 && polygon[i].y == 0)
			{
				polygon[i] = polygon[i-1];
			}
			
			if (polygon[i+1].x == 0 && polygon[i+1].y == 0)
			{
				polygon[i+1] = polygon[i];
			}
		}
		
		if (polygon[i].y <= pt.y) 
		{        
			if (polygon[i+1].y > pt.y)
			{//起点纵坐标小于终点纵坐标，且监测点坐标位于起始点纵坐标之间
				if (isLeft( polygon[i], polygon[i+1], pt) > 0) 
				{//起点与终点连线与x轴围成的夹角大于起点与终点连线与x轴围成的夹角
					++wn; 
				}
			}
		}
		else 
		{                       
			if (polygon[i+1].y <= pt.y) 
			{//起点纵坐标大于终点纵坐标，且监测点坐标位于起始点纵坐标之间
				if (isLeft( polygon[i], polygon[i+1], pt) < 0) 
				{//起点与终点连线与x轴围成的夹角小于起点与终点连线与x轴围成的夹角
					--wn; 
				}
			}
		}
	}
	if (wn==0)
		return false;
	return true;
}

CFishEyePTZHandler::CFishEyePTZHandler()
{
	InitializeCriticalSection(&m_cs);
	m_impl = 0;
}

void CFishEyePTZHandler::Draw(HWND hwin, HDC hdc)
{
	EnterCriticalSection(&m_cs);

	m_win = hwin;
	m_dc = hdc;

	CDrawer d(hwin, hdc);
	int w, h;
	d.GetWidthAndHeight(w, h);
	CPoint rectP1(w/2, 0);
	CPoint rectP2(w, h/2);
	CPoint rectP3(0, h/2);
	CPoint rectP4(w/2, h);
	CPoint rectP5(w/2, h/2);
	CPoint rectP6(w, h);
	
	d.DrawRect(rectP1, rectP2, 255, 0, 0);
	d.DrawRect(rectP3, rectP4, 0, 255, 0);
	d.DrawRect(rectP5, rectP6, 0, 0, 255);

	DrawFishEyeOneWin(1, 255, 0, 0);
	DrawFishEyeOneWin(2, 0, 255, 0);
	DrawFishEyeOneWin(3, 0, 0, 255);

	if(m_impl)
	{
		m_impl->CommitSet();
	}

	LeaveCriticalSection(&m_cs);
}

bool CFishEyePTZHandler::StartDrag(CPoint point)
{
	EnterCriticalSection(&m_cs);
	
	if(isOWin(point)) // 0窗口
	{	
		m_impl = &m_FEPimpl_0;
	}
	else
	{
		m_impl = &m_FEPimpl;
	}
	
	m_impl->StartDrag(point, m_win);
	
	LeaveCriticalSection(&m_cs);

	return true;
}

void CFishEyePTZHandler::StopDrag()
{
	EnterCriticalSection(&m_cs);
	if(m_impl)
	{
		m_impl->StopDrag();
	}
	LeaveCriticalSection(&m_cs);
}

void CFishEyePTZHandler::UpdateDrag(CPoint point)
{
	EnterCriticalSection(&m_cs);
	if(m_impl)
	{
		m_impl->UpdateDrag(point);
	}
	LeaveCriticalSection(&m_cs);
}

bool CFishEyePTZHandler::isOWin(CPoint point)
{
	CDrawer d(m_win, m_dc);
	
	int w,h;
	d.GetWidthAndHeight(w, h);

	return (point.x <= w/2 && point.y <= h/2) ? true : false;
}

void CFishEyePTZHandler::DrawFishEyeOneWin(int winid, int red, int green, int blue)
{
	CDrawer d(m_win, m_dc);
	
	int w,h;
	d.GetWidthAndHeight(w, h);
	w /= 2;
	h /= 2;

	FISHEYE_EPTZPARAM getEPTZParam;
	getEPTZParam.ePtzCmd = FISHEYEEPTZ_CMD_GET_CUR_REGION;
	getEPTZParam.winId = winid;
	getEPTZParam.arg1 = 0;

	if(!CPlayer::Instance()->FisheyeEptzUpdate(&getEPTZParam, 0))
	{
		OutputDebugStringA("FisheyeEptzUpdate failed\n");
		return;
	}

	m_FEPimpl_0.SetArg(getEPTZParam.arg4, getEPTZParam.arg5, winid-1);
	FISHEYE_POINT2D* vetexsFe = (FISHEYE_POINT2D*)(getEPTZParam.pArg);
	if(!vetexsFe)
	{
		OutputDebugStringA("vetexsFe is 0\n");
		return;
	}
	
	CPoint p1;
	CPoint p2;
	p1.x = -1;
	p1.y = -1;
	
	for(int i = 0; i < 160; i++)
	{
		if(vetexsFe[i].x == 0 && vetexsFe[i].y == 0 || vetexsFe[i].x == -1 && vetexsFe[i].y == -1)
		{
			continue;
		}
		
		p2.x = vetexsFe[i].x * w / 8191;
		p2.y = vetexsFe[i].y * h / 8191;

		m_FEPimpl_0.SetPointInfo(p2, 160 * (winid-1) + i);
		
		if(p1.x != -1 && p1.y != -1)
		{
			d.DrawLine(p1, p2, red, green, blue);
		}
		
		p1 = p2;
	}

}

CDrawer::CDrawer(HWND hwin, HDC hdc)
{
	_hwin = hwin;
	_hdc = hdc;
	
	RECT rect;
	GetWindowRect(_hwin, &rect);
	
	_width = rect.right - rect.left;
	_height = rect.bottom - rect.top;
}

void CDrawer::DrawLine(POINT p1, POINT p2, int red, int green, int blue)
{
	COLORREF cr = ((red%256)|((green%256) << 8)|((blue%256) << 16));
	HPEN hPen = CreatePen(0xffffffff, 3, cr);
	SelectObject(_hdc, hPen);
	
	MoveToEx(_hdc, p1.x, p1.y, NULL);
	LineTo(_hdc, p2.x, p2.y);
	
	DeleteObject(hPen);
}

void CDrawer::DrawRect(POINT p1, POINT p2, int red, int green, int blue)
{
	COLORREF cr = ((red%256)|((green%256) << 8)|((blue%256) << 16));
	HPEN hPen = CreatePen(0xffffffff, 3, cr);
	SelectObject(_hdc, hPen);
	
	p2.x -= 2;
	p2.y -= 2;
	MoveToEx(_hdc, p1.x, p1.y, NULL);
	
	POINT rect[5];
	rect[4].x = rect[0].x = p1.x;
	rect[4].y = rect[0].y = p1.y;
	
	rect[1].x = p2.x;
	rect[1].y = p1.y;
	
	rect[2].x = p2.x;
	rect[2].y = p2.y;
	
	rect[3].x = p1.x;
	rect[3].y = p2.y;
	
	Polyline(_hdc, rect, 5);
	
	DeleteObject(hPen);
}

void CDrawer::GetWidthAndHeight(int& width, int& height)
{
	width = _width;
	height = _height;
}
