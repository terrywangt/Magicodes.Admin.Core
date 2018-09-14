import { Component, OnInit,EventEmitter,Input,Output,Injector } from '@angular/core';
import { TreeServiceProxy } from '@shared/service-proxies/service-proxies';
import {TreeNode} from 'primeng/api';
import { AppComponentBase } from '@shared/common/app-component-base';
@Component({
  selector: 'columnInfo-tree-select',
  templateUrl: './columnInfo-tree-select.component.html',
  styleUrls: ['../tree-select.component.less']
})
export class ColumnInfoTreeSelectComponent  extends AppComponentBase implements OnInit {
  @Input() value:TreeNode={
    data:{}
  };
  @Input() level:number=4;
  @Output() valueChange:EventEmitter<any>=new EventEmitter();
  rows:number=20;
  list:TreeNode[]=[];
  isShow:boolean=false;
  loading:boolean=true;
  constructor(
    injector:Injector,
    private treeServiceProxy:TreeServiceProxy
  ) {
    super(injector)
  }
  ngOnInit() {
	this.getList(0);
  }

  getList(item){
    this.loading=true;
    this.treeServiceProxy.getColumnInfoTreeNodes(item).subscribe((result)=>{
      this.loading=false;
      if(item){
        item.children=result.data;
        this.list=[...this.list];
      }else{
        this.list=result.data;
		this.list.unshift({
          "label": "无",
          "data": {
            "isSelect":true,
            "title": "无",
            "id":undefined
          }
        });
      }
    })
  }
  /**
   * 点击显示
   */
  show(){
    this.isShow=!this.isShow;
  }
  /**
   * 打开节点
   * @param event 当前节点
   */
  open(event){
    if(!event.node.children){
      this.getList(event.node)
    }
  }
  select(item){
    //if(item.level+1==this.level){
      this.isShow=!this.isShow;
	  if(item.parent!=null){
		item.parent.children.forEach(ele => {
			ele.data.isSelect=false;
		});
	  }      
      item.node.data.isSelect=true;
      this.value=item.node;
      this.value.data.label=item.node.data.title;
      this.resovleItem(item);
      this.valueChange.emit(this.value);
      console.log(item)
    //}
  }
  /**
   * 解析层级
   * @param item 当前层次
   */
  resovleItem(item){
    if(item.parent){
      this.value.data.label=item.parent.data.title+','+this.value.data.label;
      this.resovleItem(item.parent);
    }
  }
}