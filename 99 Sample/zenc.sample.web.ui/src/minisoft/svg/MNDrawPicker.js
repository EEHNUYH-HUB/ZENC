import { GetRect ,GetCenterforRect, Guid} from "@/minisoft/MNCommon"

export default class MNDrawText{
    constructor(list,colorobj, eventer) {
        this.Eventer = eventer;
        this.ObjList = list;
        this.Name = "PICK";
        this.IsDown = false;
        
        this.StartPoint = null;
        this.BeforeRotate = null;
        this.EndPoint = null;
        this.ColorObj = colorobj;
        this.SelectedItems = new Array;

        this.SizeSubRect = null;
        this.SizeChangObj = null;

    }
    
    MouseDown(e) {
        this.IsDown = true;
        
        
        
        this.StartPoint = new Object;
        this.StartPoint.X = e.offsetX;
        this.StartPoint.Y = e.offsetY;
        this.EndPoint = new Object;
        this.EndPoint.X = e.offsetX;
        this.EndPoint.Y = e.offsetY;

        //this.AllUnSelected()

        if (this.SelectedItems.length > 0) {
            for (var i in this.SelectedItems){
                var obj = this.SelectedItems[i];
                
                if (obj && obj.Rect.SubRects) {
                    for (var j in obj.Rect.SubRects) {
                        var sub = obj.Rect.SubRects[j];
                        if (sub.IsDown) {
                            this.SizeSubRect = sub;
                            
                            this.AllUnSelected()
                            obj.IsSelected = true;
                            this.SizeChangObj = obj;
                            this.BeforeRotate = this.SizeChangObj.Rect.Rotate;
                            this.SelectedItems.push(obj);
                            return;
                        }
                    }
                }
            }
        }

        var selItem = null;
        for (var i in this.ObjList) {

            var item = this.ObjList[i];
            if (this.IsFind(item.Rect,e.offsetX, e.offsetY)) {
                selItem = item;  
                break;
            }
        }

        if (selItem) {
            if (this.SelectedItems.length > 1) {
                if (!selItem.IsSelected) {
                    this.AllUnSelected() 
                selItem.IsSelected = true;
                this.SelectedItems.push(selItem);
                }
            }
            else {
                this.AllUnSelected() 
                selItem.IsSelected = true;
                this.SelectedItems.push(selItem);
            }
                
        }
        else {
            this.AllUnSelected() 
        }
        

    }
    AllUnSelected() {
        for (var i in this.SelectedItems) {
            this.SelectedItems[i].IsSelected = false;
        }
        this.SelectedItems = new Array;
    }
   
    MouseMove(e) {
        if (this.IsDown && this.SelectedItems && this.SelectedItems.length > 0) {
        
            var end = new Object;
            end.X = e.offsetX;
            end.Y = e.offsetY;
            
            if (this.SizeSubRect) {
                this.SizeSvgObj(this.StartPoint, end)
            }
            else {
                this.MoveSvgObj(this.StartPoint, end)
            }
            
            this.StartPoint.X = end.X;
            this.StartPoint.Y = end.Y;
        }
        else  if (this.IsDown) {
            this.EndPoint.X = e.offsetX;
            this.EndPoint.Y = e.offsetY;
            
            this.DrawItem
                = " M" + this.StartPoint.X + " " + this.StartPoint.Y
                + " L" + this.EndPoint.X + " " + this.StartPoint.Y
                + " L" + this.EndPoint.X + " " + this.EndPoint.Y
                + " L" + this.StartPoint.X + " " + this.EndPoint.Y
                + " L" + this.StartPoint.X + " " + this.StartPoint.Y +" Z";
        }
    }

    MouseUp(e) {
        this.IsDown = false;
        this.DrawItem = "";
        if (this.SizeSubRect) {
            this.SizeSubRect.IsDown = false;
            this.SizeSubRect = null;
            this.SizeChangObj = null;
        }
        // /alert(this.SelectedItems.length);
        if(this.SelectedItems.length == 0){
            for (var i in this.ObjList) {

                var item = this.ObjList[i];
                if (this.IsFind2(item.Rect,this.StartPoint.X , this.StartPoint.Y ,this.EndPoint.X ,this.EndPoint.Y )) {
                    
                    item.IsSelected = true;
                    this.SelectedItems.push(item);
                    
                }
            }
        }

        this.Eventer.SelectedMethod(this.SelectedItems);
    }
    SizeSvgObj(start, end) {

        
        if (this.SizeSubRect.Type == "TR") {


            
            var cp = GetCenterforRect(this.SizeChangObj.Rect);
            
            var cx = cp.X;
            var cy = cp.Y;
            var x = cx - end.X;
            var y = cy - end.Y;
            
            var r = - ((Math.atan(x / y) * (180 / Math.PI)));
            
            if (y < 0) { 
                
                r = (180 + r)
            }

            
            this.SizeChangObj.Rect.Rotate =  r; //*(180/Math.PI);
            
        }
        else {
            if (this.SizeSubRect.Type.indexOf('R') != -1) {
                var newWidth = (this.SizeChangObj.Rect.Width * this.SizeChangObj.Rect.ScaleX) + end.X - start.X;
                this.SizeChangObj.Rect.ScaleX = newWidth / this.SizeChangObj.Rect.Width;
            }
            if (this.SizeSubRect.Type.indexOf('L') != -1) {
                var newWidth = (this.SizeChangObj.Rect.Width * this.SizeChangObj.Rect.ScaleX) - (end.X - start.X);
                this.SizeChangObj.Rect.X = end.X
                this.SizeChangObj.Rect.ScaleX = newWidth / this.SizeChangObj.Rect.Width;
            }
            if (this.SizeSubRect.Type.indexOf('B') != -1) {
                var newHeight = (this.SizeChangObj.Rect.Height * this.SizeChangObj.Rect.ScaleY) + end.Y - start.Y;
                this.SizeChangObj.Rect.ScaleY = newHeight / this.SizeChangObj.Rect.Height;
            }
            if (this.SizeSubRect.Type.indexOf('T') != -1) {
                var newHeight = (this.SizeChangObj.Rect.Height * this.SizeChangObj.Rect.ScaleY) - (end.Y - start.Y);
            
                this.SizeChangObj.Rect.Y = end.Y
                this.SizeChangObj.Rect.ScaleY = newHeight / this.SizeChangObj.Rect.Height;
            }
        }

         this.Eventer.ChangedMethod(this.SelectedItems);
    }
    MoveSvgObj(start,end) {
        for (var i in this.SelectedItems) {
            var obj = this.SelectedItems[i];
            obj.Rect.X = obj.Rect.X + end.X - start.X;
            obj.Rect.Y = obj.Rect.Y + end.Y - start.Y;
            obj.Rect.X2 = obj.Rect.X2 + end.X - start.X;
            obj.Rect.Y2 = obj.Rect.Y2 + end.Y - start.Y;

        }

        this.Eventer.ChangedMethod(this.SelectedItems);
        
    }

    ChangedColor(ColorObj) {
        if (this.SelectedItems && this.SelectedItems.length > 0) {
            for (var i in this.SelectedItems) {
                var obj = this.SelectedItems[i];
                obj.StrokeColor = ColorObj.Stroke;
                obj.FillColor = ColorObj.Fill;
            }
            this.Eventer.ChangedMethod(this.SelectedItems);
        }
    }

    DeleteSvgObj() {
        var deleteItems = new Array;
        if (this.SelectedItems && this.SelectedItems.length > 0) {
            for (var i in this.SelectedItems) {
                var obj = this.SelectedItems[i];
                obj.IsSelected = false;
                
                for (var j in this.ObjList) {
                    if (this.ObjList[j].ID == obj.ID) {
                        this.ObjList.splice(j, 1);
                        deleteItems.push(obj);            
                        break;
                    }
                }
                
            }
        }
        
        this.AllUnSelected()    
        this.Eventer.RemovedMethod(deleteItems);
    }
    IsFind(rect, x, y) {
        if (rect  && x && y) {
            var x1 = rect.X;
            var y1 = rect.Y;
            var x2 = (rect.Width * rect.ScaleX) + rect.X;
            var y2 = (rect.Height * rect.ScaleY) + rect.Y;
        
            var minX = x1 > x2 ? x2 : x1;
            var minY = y1 > y2 ? y2 : y1;
            var maxX = x1 > x2 ? x1 : x2;
            var maxY = y1 > y2 ? y1 : y2;
        
            return (minX < x && maxX > x && minY < y && maxY > y);
        }
        return false;
    }
    IsFind2(rect, x1, y1, x2, y2) {
        if (rect && x1 && y1 && x2 && y2) {
            
            var minX = x1 > x2 ? x2 : x1;
            var minY = y1 > y2 ? y2 : y1;
            var maxX= x1 > x2 ? x1  : x2 ;
            var maxY= y1 > y2 ? y1  : y2 ;    
            var cp = GetCenterforRect(rect);
        
            return (cp.X > minX && cp.X < maxX && cp.Y > minY && cp.Y < maxY);
        }
        return false;
    }

    
}
