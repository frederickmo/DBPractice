<template>
  <el-container style="height: 100%; border: 1px solid #eee">
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
          <el-menu-item index="2-2">交接记录查看</el-menu-item>
        </el-submenu>
        <el-menu-item index="3" route><i class="el-icon-circle-close"></i>退出系统</el-menu-item>
      </el-menu>
    </el-aside>
    <!--两个右上角的按钮对应开始新的运输和更改车辆-->
    <el-container>
      <el-header style="text-align: right; font-size: 12px">
        <span>{{ username }}</span>
      </el-header>
      <!--主体内容显示-->
      <el-main>
        <GetInfo v-if="this.choice===1"></GetInfo>
        <UpdateInfo v-if="this.choice===2"></UpdateInfo>
        <UpdatePassword v-if="this.choice===3"></UpdatePassword>
        <GetTransportRecord v-if="this.choice===4"></GetTransportRecord>
        <GetUnfinished v-if="this.choice===5"></GetUnfinished>
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
import GetInfo from "@/components/StationStaff/GetInfo";
import UpdateInfo from "@/components/StationStaff/UpdateInfo"
import UpdatePassword from "@/components/StationStaff/UpdatePassword";
import GetTransportRecord from "@/components/StationStaff/GetTransportRecord";
//import GetUnfinished from "@/components/Carrier/getUnfinished";
import {Base64} from 'js-base64';

export default {
  components: {GetInfo, UpdateInfo, UpdatePassword, GetTransportRecord},
  data() {
    return {
      choice: 1,
      username: Base64.decode(localStorage.getItem("username")),
      formLabelWidth: '120px',
      dialogFormVisible: false,//控制 选择车辆弹窗
      addTransport: false//控制 选择起点弹窗，选择起点的弹窗点外面和点x都可以关掉
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
  },
  mounted() {
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
