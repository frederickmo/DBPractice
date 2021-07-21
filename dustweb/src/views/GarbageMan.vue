<template>
  <el-container style="height: 100%; border: 1px solid #eee">
    <el-dialog
        title="新增垃圾"
        v-model="dialogFormVisible"
    >
      <!--:close-on-click-modal 问你点外面可不可以关-->
      <!--:show-close 问你需不需要显示x按钮-->
      <el-form :model="gar_type">
        <el-form-item label="垃圾类型" :label-width="formLabelWidth">
          <el-select v-model="gar_type" placeholder="请选择垃圾类型">
            <el-option v-for="(item,index) in GAR_TYPE" :key="index" :label="item"
                       :value="item"></el-option>
          </el-select>
        </el-form-item>
      </el-form>
      <template #footer>
    <span class="dialog-footer">
      <el-button type="primary" @click="addGarbage">确 定</el-button>
    </span>
      </template>
    </el-dialog>
    <el-aside width="200px" style="background-color: rgb(238, 241, 246)">
      <el-menu :default-openeds="['1']" @select="changeWindow">
        <el-submenu index="1">
          <template #title><i class="el-icon-user"></i>用户信息</template>
          <!--            <template #title>分组一</template>-->
          <el-menu-item index="1-1">查看信息</el-menu-item>
          <el-menu-item index="1-2">修改信息</el-menu-item>
          <el-menu-item index="1-3">修改密码</el-menu-item>
        </el-submenu>
        <el-submenu index="2">
          <template #title><i class="el-icon-menu"></i>记录查看</template>
          <el-menu-item index="2-1">投放记录查看</el-menu-item>
          <el-menu-item index="2-2">违规记录查看</el-menu-item>
        </el-submenu>
        <el-menu-item index="3" route><i class="el-icon-circle-close"></i>退出系统</el-menu-item>
      </el-menu>
    </el-aside>

    <el-container>
      <el-header style="text-align: right; font-size: 12px">
        <el-button icon="el-icon-plus" style="margin-right: 15px" circle @click="dialogFormVisible=1"></el-button>
        <span>{{ username }}</span>
      </el-header>

      <el-main>
        <GetInfo v-if="this.choice===1"></GetInfo>
        <UpdateInfo v-if="this.choice===2"></UpdateInfo>
        <UpdatePassword v-if="this.choice===3"></UpdatePassword>
        <GetRecord v-if="this.choice===4"></GetRecord>
        <GetViolate v-if="this.choice===5"></GetViolate>
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
import GetInfo from "@/components/GarbageMan/GetInfo";
import UpdateInfo from "@/components/GarbageMan/UpdateInfo"
import UpdatePassword from "@/components/GarbageMan/UpdatePassword";
import GetRecord from "@/components/GarbageMan/GetRecord";
import GetViolate from "@/components/GarbageMan/GetViolate";
import {Base64} from 'js-base64';

export default {
  components: {GetInfo, UpdateInfo, UpdatePassword, GetRecord, GetViolate},
  data() {
    return {
      choice: 1,
      formLabelWidth: '120px',
      username: Base64.decode(localStorage.getItem("username")),
      GAR_TYPE:[
        "干垃圾",
        "湿垃圾",
        "有害垃圾",
        "可回收垃圾"
      ],
      gar_type:"",
      dialogFormVisible:false
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
    addGarbage(){
      fetch(this.$store.state.URL + "/Garbage/Add?req=" + encodeURIComponent(this.gar_type), {
        method: "GET",
        headers: {
          "Authorization": "Bearer " + Base64.decode(localStorage.getItem("token")),
        },
      }).then((response) => {
        let result = response.json();
        result.then((result) => {
          console.log(result)
          this.dialogFormVisible=false;
        })
      })
    }
  }
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
