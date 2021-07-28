<template>
  <div>
    <el-row class="row-bg" justify="left">
      <el-col :span="3" :offset="2">
        <div class="grid-content bg-purple">账号</div>
      </el-col>
      <el-col :span="3">
        <div class="grid-content bg-purple-light">{{ UserInfo.account }}</div>
      </el-col>
    </el-row>
    <el-row class="row-bg" justify="left">
      <el-col :span="3" :offset="2">
        <div class="grid-content bg-purple">姓名</div>
      </el-col>
      <el-col :span="3">
        <div class="grid-content bg-purple-light">{{ UserInfo.name }}</div>
      </el-col>
    </el-row>
    <el-row class="row-bg" justify="left">
      <el-col :span="3" :offset="2">
        <div class="grid-content bg-purple">工作地点</div>
      </el-col>
      <el-col :span="3">
        <div class="grid-content bg-purple-light">{{ UserInfo.plantname}} </div>
      </el-col>
    </el-row>
    <el-row class="row-bg" justify="left">
      <el-col :span="3" :offset="2">
        <div class="grid-content bg-purple">联系电话</div>
      </el-col>
      <el-col :span="3">
        <div class="grid-content bg-purple-light">{{ UserInfo.phone }}</div>
      </el-col>
    </el-row>
  </div>
</template>
<script>
import { Base64 } from 'js-base64';
export default {
  data() {
    return {
      //用户信息
      UserInfo: {
        account: Base64.decode(localStorage.getItem("username")),
        name: '',
        phone: '',
        plant_name:'',
        password: Base64.decode(localStorage.getItem("password")),
      }
    };
  },
  mounted() {
    console.log(this.UserInfo.account, this.UserInfo.password);
    fetch(this.$URL + "/User/GetInformation/StationStaff?req=" + this.UserInfo.account, {
      method: "GET",
      headers: {
        "Authorization": "Bearer " + Base64.decode(localStorage.getItem("token")),
      },
    }).then((response) => {
      let result = response.json();
      result.then((result) => {
        this.UserInfo.name = result.name;
        this.UserInfo.phone = result.phonenumber;
        this.UserInfo.address = result.address;
        this.UserInfo.plantname=result.plantname;
      })
    })
  }
}
</script>
<style>
.el-row {
  margin-bottom: 20px;
}

.el-row:last-child {
  margin-bottom: 0;
}

.el-col {
  border-radius: 4px;
}

.bg-purple-dark {
  background: #99a9bf;
}

.bg-purple {
  background: #d3dce6;
}

.bg-purple-light {
  background: #e5e9f2;
}

.grid-content {
  border-radius: 4px;
  min-height: 36px;
}

.row-bg {
  padding: 10px 0;
  background-color: #f9fafc;
}

</style>