<template>
  <div>
    <transition name="el-fade-in-linear">
      <el-alert
          title="修改成功"
          type="success"
          :closable="false"
          style="width: 500px"
          show-icon v-if="UpdateStatus">
      </el-alert>
    </transition>
    <div style="margin: 20px;"></div>
    <el-form :label-position="labelPosition" label-width="80px" ref="UserInfo" :model="UserInfo" style="width: 500px">
      <el-form-item label="姓名" prop="name">
        <el-input v-model="UserInfo.name"></el-input>
      </el-form-item>
      <el-form-item label="联系电话" prop="phone">
        <el-input v-model="UserInfo.phone"></el-input>
      </el-form-item>
      <el-form-item label="工作地点" prop="workingStation">
      <el-select v-model="UserInfo.workingStation" placeholder="请选择工作地点" style="width: 420px">
        <el-option v-for="(item,index) in stationData" :key="index" :label="item.name"
                   :value="item.name"
        ></el-option>
      </el-select>
      </el-form-item>
      <el-form-item label="地址" prop="address">
        <el-input v-model="UserInfo.address"></el-input>
      </el-form-item>
      <el-form-item>
        <el-button type="primary" @click="submitForm">提交</el-button>
        <el-button @click="resetForm('UserInfo')">重置</el-button>
      </el-form-item>
    </el-form>
  </div>
</template>

<script>
import { Base64 } from 'js-base64';
export default {
  data() {
    return {
      labelPosition: 'right',
      //用户信息
      UserInfo: {
        account: Base64.decode(localStorage.getItem("username")),
        name: '',
        workingStation: Base64.decode(localStorage.getItem("workingStation")),
        phone: '',
        address: '',
        password: Base64.decode(localStorage.getItem("password")),
      },
      stationData: [],
      UpdateStatus: false
    };
  },
  methods: {
    submitForm() {
      const req= {
        "id": this.UserInfo.account,
        "name": this.UserInfo.name,
        "password": "",
        "phonenumber": this.UserInfo.phone,
        "address": this.UserInfo.address
      }
      localStorage.setItem("workingStation",Base64.encode(this.UserInfo.workingStation));
      console.log(this.UserInfo.account, this.UserInfo.password);
      fetch(this.$store.state.URL + "/User/Update/Watcher", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          "Authorization": "Bearer " + Base64.decode(localStorage.getItem("token"))
        },
        body: JSON.stringify(req),
      }).then((response) => {
        console.log(this.UserInfo);
        let result = response.json();
        result.then((res) => {
              console.log(res);
              this.UpdateStatus = res.status;
            }
        )
      })
    },
    resetInfo(){
      this.UserInfo.workingStation=Base64.decode(localStorage.getItem("workingStation"));
      fetch(this.$store.state.URL + "/User/GetInformation/Watcher?req=" + this.UserInfo.account, {
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
    },
    resetForm(formName) {
      this.$refs[formName].resetFields();
      this.resetInfo();
    }
  },
  mounted() {
    this.UpdateStatus = false;
    this.resetInfo();
    fetch(this.$store.state.URL + "/Facility/Binsite/GetAll", {
      method: "GET",
      headers: {
        "Authorization": "Bearer " + Base64.decode(localStorage.getItem("token")),
      },
    }).then((response) => {
      let result = response.json();
      result.then((result) => {
        this.stationData = result;
      })
    })
  }
}
</script>

<style>

</style>
