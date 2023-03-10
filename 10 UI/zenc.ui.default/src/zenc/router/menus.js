import {h,ref} from 'vue'
import { RouterLink  } from 'vue-router'

function ConvertToMenuItem(currentDept,allowDept,parent,menuJson,pagePath,findActiveIdHandler){
    
    var obj = new Object;
    obj.key = menuJson.id;
    obj.desc= menuJson.desc;
    var strPath = parent+menuJson.path;
    var iof = pagePath.indexOf(strPath);
    if(iof > -1){
        //console.log(iof,strPath,pagePath,menuJson.id)
        findActiveIdHandler(menuJson.id);
    }
    if(menuJson.children && menuJson.children.length > 0){
        
        if(currentDept < allowDept){
            obj.children = new Array;
            for(var i in menuJson.children){    
                obj.children.push(ConvertToMenuItem(currentDept+1,allowDept,strPath+"/",menuJson.children[i],pagePath,findActiveIdHandler));
            }
        }
    }
    if(menuJson.component)
        obj.label = () =>h( RouterLink, { to: { name: menuJson.path } }, { default: () => menuJson.name });
    else{
        obj.type = "group";
        obj.label = () => menuJson.name ;
    }

    return obj;
}

export function GetBreadcrumbObjRef(route){
    
    var pagePath = route.fullPath;
    var paths = pagePath.split('/');
    var rtn = new Array;
    var len = paths.length;
    var targetList =  menuJsonList;
    var beforePath = "/";
    for(var i =1;i<len;i++){
        var currentPath = paths[i];
        for(var j in targetList){
            var tmp = targetList[j];
            if(tmp.path == currentPath){
                rtn.push({ name : tmp.name ,path:tmp.path ,desc:tmp.desc});
                targetList = tmp.children;
                break;
            }
        }
    }
    
    return ref(rtn);
}

export function GetMenuObjRef(startIndex,allowDept,route) {
    var rtn = new Object;
    rtn.Menus = new Array;
    rtn.ActiveKey =""

    if(allowDept > 0){
        var pagePath = route.fullPath;
        var paths = pagePath.split('/');
        
        
        var len = paths.length-1;
        var targetList = null;
        var beforePath = "/";
        var currentIndex = 0;
        if(len > startIndex){
            
            targetList =  menuJsonList;
            if(startIndex != currentIndex){
                currentIndex+=1;
                for(var i =1;i<len;i++){
                    var currentPath = paths[i];
                    var isOK = false;
                    
                    for(var j in targetList){
                        var tmp = targetList[j];
                        if(tmp.path == currentPath){
                            targetList = tmp.children;
                            beforePath += tmp.path+"/";
                            isOK = true;
                            break;
                        }
                    }

                    if(isOK){
                        isOK = false;
                        if(startIndex == currentIndex){
                            break;
                        }
                        currentIndex +=1;
                    }else{
                        targetList = null;
                        break;
                    }
                
                }
            }
        }

        if(targetList){
            for(var i in targetList){    
                rtn.Menus.push(ConvertToMenuItem(1,allowDept,beforePath,targetList[i],pagePath,(id)=>{
                    rtn.ActiveKey = id
                }));
            }

        }
    }

    return ref(rtn);
}

export function GetRoutes () {
    
    var result =  ConvertToRoutes(menuJsonList,true,"");

    return result;
}

function ConvertToRoutes(lst,isFirst,beforePath){
    var rtn = new Array;

    for(var i in lst){
        var item = lst[i];
    
        
        var strPath = (isFirst?'/':'')+item.path; 
        if(isFirst){
            rtn.push({
                
                path: "/",
                redirect :strPath
                
            });
        }
        if(item.children && item.children.length >0){
           
            var redirectPath = beforePath? beforePath+'/'+strPath:strPath;
            var cItems =ConvertToRoutes(item.children,false,redirectPath);
            rtn.push({
                
                path: strPath,
                name :item.path,
                component: item.component,
                redirect :redirectPath+'/'+ cItems[0].path,
                children : cItems
            });
        }
        else{
            rtn.push({
                path: strPath,
                name :item.path,
                component: item.component
            });
        }
    }

    return rtn;
}

const menuJsonList =
[
    {
        id:"0"
        ,name:"Developer Advanced Setting"
        ,path:"dasapi"
        ,desc: "?????? ??? ???????????? ?????? ??????"
        ,component: () => import("@/zenc/layout/LY0001.vue")
        ,children:[
            {
                id:"0-1"
                ,name:".Net 6.0 ?????? ??????"
                ,path:"be"       
                ,desc: ".Net 6.0 ?????? ?????? ?????? ??? ?????? ??????"                   
                ,component : () => import("@/views/D0001.vue")                      
            }
            , {
                id:"0-2"
                ,name:"Vue3.0 ?????? ??????"
                ,path:"fe"       
                ,desc: "Vue3.0 ?????? ?????? ?????? ??? ?????? ??????"         
                ,component : () => import("@/views/D0001.vue")                      
            }
        ]
    },
    {
        id:"1"
        ,name:"Rest API Sample"
        ,path:"rstapi"
        ,desc: "Rest API Sample??? ???????????? ??????"
        ,component: () => import("@/zenc/layout/LY0001.vue")
        ,children:[
            {
                id:"1-1"
                ,name:"Auth API ??????"
                ,path:"authapi"       
                ,desc: "Auth API ?????? ??????"         
                ,children:[
                    {
                        id:"1-1-1"
                        ,name:"Certification"
                        ,path:"gcmethod"
                        ,desc: "?????? ??????"         
                        ,component : () => import("@/views/T0001.vue")                      
                                   
                    }      
                ]                
                           
            },
            {
                id:"1-2"
                ,name:"DataBase API ??????"
                ,path:"dbapi"                
                ,children:[
                    {
                        id:"1-2-1"
                        ,name:"SmartSQL"
                        ,path:"smsql"
                        ,desc: "MyBatis in C# + .NET Core + Cache ( Memory|Redis )"         
                        ,component : () => import("@/views/T0002.vue")                      
                                   
                    }      
                    ,{
                        id:"1-2-2"
                        ,name:"Dynamic Procedure"
                        ,path:"dp"
                        ,desc: "Stored procedure ??????"         
                        ,component : () => import("@/views/T0003.vue")                      
                                   
                    }    
                ]                
                           
            },
            {
                id:"1-3"
                ,name:"Dynamic API ?????? "
                ,path:"dyapi"
                ,desc: "?????? DLL ????????? ?????? ??????"         
                ,children:[
                    {
                        id:"1-3-1"
                        ,name:"LoadAssembly"
                        ,path:"lamethod"
                        ,desc: "?????? DLL ????????? ?????? ??????"         
                        ,component : () => import("@/views/T0001.vue")                 
                    }       
                    
                ]                    
                           
            }
            ,
            {
                id:"1-4"
                ,name:"File API ?????? "
                ,path:"fileapi"
                ,desc: "?????? ????????? ???????????? ??????"     
                ,component : () => import("@/views/F0001.vue")                 
            }
        ]
    }
    ,
    {
        id:"2"
        ,name:"Naive UI Sample"
        ,path:"Comp"
        ,desc: "UI ?????? ?????????"         
        ,component: () => import("@/zenc/layout/LY0001.vue")
    }
    ,
    {
        id:"3"
        ,name:"Azure API Sample"
        ,path:"azapi"
        ,desc: "Azure API Sample??? ???????????? ??????"
        ,component: () => import("@/zenc/layout/LY0001.vue")
        ,children:[
            {
                id:"3-1"
                ,name:"Azure Authorization"
                ,path:"azauth"       
                ,desc: "Azure ?????? ?????? ??????"         
                ,children:[
                    {
                        id:"3-1-1"
                        ,name:"OAuth2"
                        ,path:"azoauth"
                        ,desc: "Azure OAuth2 ??????"         
                        ,component : () => import("@/views/A0001.vue")                      
                                   
                    }      
                ]                
                           
            }
        ]
    }

]


    