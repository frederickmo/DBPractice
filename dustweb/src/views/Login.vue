<template>
  <el-container class="outer" direction="vertical">
    <el-row justify="center" align="middle">
      <el-alert
          title="登陆成功"
          type="success"
          :closable="false"
          style="width: 300px"
          show-icon v-if="LoginStatus==1">
      </el-alert>
      <el-alert
          title="登陆失败"
          type="error"
          :closable="false"
          style="width: 300px"
          show-icon v-if="LoginStatus==-1">
      </el-alert>
    </el-row>
    <br/>
    <el-row justify="center" align="middle">
      <el-row justify="center"
              style="width: 300px;background: lightskyblue;border-radius: 20px; box-shadow: 0 2px 4px rgba(0, 0, 0, .12), 0 0 6px rgba(0, 0, 0, .04)">
        <el-row justify="center" style="height: 300px" align="bottom">
          <el-form :model="LoginForm" status-icon :rules="rules" ref="LoginForm" label-width="0px" class="LoginForm">
            <h1>登陆</h1>
            <el-form-item prop="username">
              <el-input type="username" v-model="LoginForm.username" autocomplete="on" placeholder="用户名"></el-input>
            </el-form-item>
            <el-form-item prop="password">
              <el-input type="password" v-model="LoginForm.password" autocomplete="on" placeholder="密码"></el-input>
            </el-form-item>
            <el-form-item>
              <el-button type="primary" round @click="submitForm('LoginForm')">提交</el-button>
              <el-button round @click="resetForm('LoginForm')">重置</el-button>
            </el-form-item>
          </el-form>
        </el-row>
      </el-row>
    </el-row>
  </el-container>
</template>
<script>
import {Base64} from 'js-base64';

export default {
  data() {
    let validateUsername = (rule, value, callback) => {
      if (value === '') {
        callback(new Error('请输入用户名'));
      } else {
        callback();
      }
    };
    let validatePass = (rule, value, callback) => {
      if (value === '') {
        callback(new Error('请输入密码'));
      } else {
        callback();
      }
    };
    return {
      LoginForm: {
        username: "",
        password: "",
      },
      rules: {
        username: [
          {validator: validateUsername, trigger: 'blur'}
        ],
        password: [
          {validator: validatePass, trigger: 'blur'}
        ],
      },
      LoginStatus: 0,
    };
  }, methods: {

    submitForm(formName) {
      this.$refs[formName].validate((valid) => {
        if (valid) {
          //console.log(this.$store.state.URL)
          const req = {userID: this.LoginForm.username, password: this.LoginForm.password};
          fetch(this.$store.state.URL + "/User/Login", {
            method: "POST",
            headers: {"Content-Type": "application/json"},
            body: JSON.stringify(req),
          }).then((response) => {
            let result = response.json();
            result.then(res => {
              if (res.status === 1) {
                this.LoginStatus = 1;
                localStorage.setItem("username", Base64.encode(this.LoginForm.username));
                localStorage.setItem("password", Base64.encode(this.LoginForm.password));
                localStorage.setItem("token", Base64.encode(res.token));
                switch (res.role) {
                  case("GarbageMan"):
                    this.$router.push('/GarbageMan');
                    break;
                  case("Watcher"):
                    this.$router.push('/Watcher');
                    break;
                  case("Carrier"):
                    this.$router.push('/Carrier');
                    break;
                  case("StationStaff"):
                    this.$router.push('/StationStaff');
                    break;
                }
              } else {
                this.LoginStatus = -1;
              }
              console.log(res.token, this.LoginForm.username, this.LoginForm.password);
            })
          })
        } else {
          console.log('error submit!!');
          return false;
        }
      });
    },
    resetForm(formName) {
      this.$refs[formName].resetFields();
    }
  },
  mounted() {
    console.log(this.LoginStatus);
    this.LoginStatus = 0;
    localStorage.clear();
  }
}
</script>
<style>
body, html {
  margin: 0px;
  padding: 0px;
  height: 100%;
  width: 100%;
}

.outer {
  height: 100%;
  width: 100%;
}

</style>