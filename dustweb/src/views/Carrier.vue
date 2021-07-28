<!--逻辑解释：-->
<!--当一个carrier没有选择他开的车辆的时候，那么就要跳一个对话框出来让他必须选一个，这个放在mounted里弄-->
<!--当一个carrier想换车的时候，那么这个对话框也要跳出来，这个在框架里放一个按钮，弹同一个对话框-->
<!--当他要开始运输的时候，直接弹一个对话框让他选起始的地点-->
<!--但其实可以用上二维码让他扫，这个要写手机端的app-->
<template>
  <el-container style="height: 100%; border: 1px solid #eee">
    <!--选择当前运输车辆的对话框-->
    <el-dialog
        :close-on-click-modal='false'
        title="工作地点"
        :show-close='false'
        v-model="dialogFormVisible"
    >
    <!--:close-on-click-modal 问你点外面可不可以关-->
    <!--:show-close 问你需不需要显示x按钮-->
      <el-form :model="Truck">
        <el-form-item label="工作车辆" :label-width="formLabelWidth">
          <el-select v-model="Truck" placeholder="请选择工作车辆">
            <el-option v-for="(item,index) in TruckData" :key="index" :label="item"
                       :value="item"></el-option>
          </el-select>
        </el-form-item>
      </el-form>
      <template #footer>
    <span class="dialog-footer">
      <el-button type="primary" @click="setTruck">确 定</el-button>
    </span>
      </template>
    </el-dialog>
    <!--选择开始运输出发点的对话框-->
    <el-dialog
        title="开始运输"
        v-model="addTransport"
    >
      <el-form :model="dustbin">
        <el-form-item label="选择起点" :label-width="formLabelWidth">
          <el-select v-model="dustbin" placeholder="运输起点">
            <el-option v-for="(item,index) in DustbinData" :key="index" :label="item.id"
                       :value="item.id"></el-option>
          </el-select>
        </el-form-item>
      </el-form>
      <template #footer>
    <span class="dialog-footer">
      <el-button type="primary" @click="startTransport">确 定</el-button>
    </span>
      </template>
    </el-dialog>
    <!--目录式菜单框架-->
    <el-aside width="200px" style="background-color: rgb(238, 241, 246)">
      <el-menu :default-openeds="['1']" @select="changeWindow">
        <el-submenu index="1">
          <template #title><i class="el-icon-user"></i>用户信息</template>
          <el-menu-item index="1-1">查看信息</el-menu-item>
          <el-menu-item index="1-2">修改信息</el-menu-item>
          <el-menu-item index="1-3">修改密码</el-menu-item>
        </el-submenu>
        <el-submenu index="2">
          <template #title><i class="el-icon-menu"></i>记录查看</template>
          <el-menu-item index="2-1">运输记录查看</el-menu-item>
          <el-menu-item index="2-2">待定,嘿嘿</el-menu-item>
        </el-submenu>
        <el-menu-item index="3" route><i class="el-icon-circle-close"></i>退出系统</el-menu-item>
      </el-menu>
    </el-aside>
    <!--两个右上角的按钮对应开始新的运输和更改车辆-->
    <el-container>
      <el-header style="text-align: right; font-size: 12px">
        <el-button icon="el-icon-plus" style="margin-right: 10px" circle @click="selectStart"></el-button>
        <el-button icon="el-icon-setting" style="margin-right: 15px" circle @click="dialogFormVisible=1"></el-button>
        <span>{{ username }}</span>
      </el-header>
      <!--主体内容显示-->
      <el-main>
        <GetInfo v-if="this.choice===1"></GetInfo>
        <UpdateInfo v-if="this.choice===2"></UpdateInfo>
        <UpdatePassword v-if="this.choice===3"></UpdatePassword>
        <GetTransportRecord v-if="this.choice===4"></GetTransportRecord>
        <!-- <GetUnfinished v-if="this.choice===5"></GetUnfinished> -->
        <!--          <el-table-column prop="date" label="日期" width="140">-->
        <!--          </el-table-column>-->
        <!--          <el-table-column prop="name" label="姓名" width="120">-->
        <!--          </el-table-column>-->
        <!--          <el-table-column prop="address" label="地址">-->
        <!--          </el-table-column>-->
        <!--        </el-table>-->
      </el-main>
    </el-container>
  </el-container>
</template>

<script>
import GetInfo from "@/components/Carrier/GetInfo";
import UpdateInfo from "@/components/Carrier/UpdateInfo"
import UpdatePassword from "@/components/Carrier/UpdatePassword";
import GetTransportRecord from "@/components/Carrier/GetTransportRecord";
//import GetUnfinished from "@/components/Carrier/getUnfinished";
import {Base64} from 'js-base64';

export default {
  components: {GetInfo, UpdateInfo, UpdatePassword, GetTransportRecord},
  data() {
    return {
      choice: 0,
      username: Base64.decode(localStorage.getItem("username")),
      Truck: String(),
      formLabelWidth: '120px',
      TruckData: [],
      DustbinData:[],
        dustbin:"",
      dialogFormVisible: false,//控制 选择车辆弹窗
      addTransport:false//控制 选择起点弹窗，选择起点的弹窗点外面和点x都可以关掉
    }
  },
  methods: {
    changeWindow(index, indexPath) {
      console.log(1, indexPath)
      console.log(2, index);
      switch (index) {
        case("1-1"):
          this.choice = 1;
          break;
        case("1-2"):
          this.choice = 2;
          break;
        case("1-3"):
          this.choice = 3;
          break;
        case("2-1"):
          this.choice = 4;
          break;
        case("2-2"):
          this.choice = 5;
          break;
        case("3"):
          this.$router.push('/');
          break;
      }
    },
    setTruck() {
      if (this.Truck === '')
        return;
      localStorage.setItem("Truck", Base64.encode(this.Truck));
      fetch(this.$URL + "/Facility/Truck/SetTruck?req=" + this.Truck, {
        method: "GET",
        headers: {
          "Authorization": "Bearer " + Base64.decode(localStorage.getItem("token")),
        },
      }).then((response) => {
        let result = response.json();
        result.then((result) => {
          console.log(result)
        })
      })
      this.dialogFormVisible = false;
      this.choice = 1;
    },
    getFree() {
      fetch(this.$URL + "/Facility/Truck/GetFree", {
        method: "GET",
        headers: {
          "Authorization": "Bearer " + Base64.decode(localStorage.getItem("token")),
        },
      }).then((response) => {
        let result = response.json();
        result.then((result) => {
          console.log(result)
          this.TruckData = result;
        })
      })
    },
    async getTruck() {
      this.Truck = await (await fetch(this.$URL + "/Facility/Truck/GetTruck", {
        method: "GET",
        headers: {
          "Authorization": "Bearer " + Base64.decode(localStorage.getItem("token")),
        }
      })).text();
      if (this.Truck === "") {
        this.choice = 0;
        this.dialogFormVisible = 1;
      } else {
        this.choice = 1;
        this.dialogFormVisible = 0;
        localStorage.setItem("Truck", Base64.encode(this.Truck));
      }

    },
    async selectStart() {
      this.DustbinData = await (await fetch(this.$URL + "/Facility/TrashCan/GetAll", {
        method: "GET",
        headers: {
          "Authorization": "Bearer " + Base64.decode(localStorage.getItem("token")),
        }
      })).json();
      this.addTransport = true;
    },
    startTransport()
    {
      const req = {
        truck_id: Base64.decode(localStorage.getItem("Truck")),
        dustbin_id: this.dustbin
      };
      fetch(this.$URL + "/Transport/TransportStart", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          "Authorization": "Bearer " + Base64.decode(localStorage.getItem("token")),
        },
        body:JSON.stringify(req)
      }).then((response) => {
        let result = response.json();
        result.then((result) => {
          console.log(result)
        })
      })
      this.addTransport = false;
    }
  },
  mounted() {
    //getTruck 看一眼这个驾驶员有没有坐骑，没有的话弹个窗把无主的车给他选一个，不选不能关
    this.getTruck();
    //获取一下无主的车的列表
    this.getFree();
  },
};
</script>

<style>
html, body, #app, .el-container {
  /*设置内部填充为0，几个布局元素之间没有间距*/
  padding: 0px;
  /*外部间距也是如此设置*/
  margin: 0px;
  /*统一设置高度为100%*/
  height: 100%;
}

.el-header {
  background-color: #B3C0D1;
  color: #333;
  line-height: 60px;
}

.el-aside {
  color: #333;
}
</style>
