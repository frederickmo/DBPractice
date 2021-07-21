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
        <div class="grid-content bg-purple">信用积分</div>
      </el-col>
      <el-col :span="3">
        <div class="grid-content bg-purple-light">{{ UserInfo.credit }}</div>
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
    <el-row class="row-bg" justify="left">
      <el-col :span="3" :offset="2">
        <div class="grid-content bg-purple">地址</div>
      </el-col>
      <el-col :span="3">
        <div class="grid-content bg-purple-light">{{ UserInfo.address }}</div>
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
        credit: '',
        phone: '',
        address: '',
        password: Base64.decode(localStorage.getItem("password")),
      }
    };
  },
  mounted() {
    console.log(this.UserInfo.account, this.UserInfo.password);
    fetch(this.$store.state.URL + "/User/GetInformation/GarbageMan?req=" + this.UserInfo.account, {
      method: "GET",
      headers: {
        "Authorization": "Bearer " + Base64.decode(localStorage.getItem("token")),
      },
    }).then((response) => {
      let result = response.json();
      result.then((result) => {
        this.UserInfo.name = result.name;
        this.UserInfo.credit = result.credit;
        this.UserInfo.phone = result.phonenumber;
        this.UserInfo.address = result.address;
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