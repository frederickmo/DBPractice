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
    <el-form :model="UpdateForm" status-icon :rules="rules" ref="UpdateForm" label-width="100px" class="demo-ruleForm" style="width: 500px">
      <el-form-item label="原密码" prop="oldPass">
        <el-input type="password" v-model="UpdateForm.oldPass" autocomplete="off"></el-input>
      </el-form-item>
      <el-form-item label="新密码" prop="newPass">
        <el-input type="password" v-model="UpdateForm.newPass" autocomplete="off"></el-input>
      </el-form-item>
      <el-form-item label="确认密码" prop="checkPass">
        <el-input type="password" v-model="UpdateForm.checkPass" autocomplete="off"></el-input>
      </el-form-item>
      <el-form-item>
        <el-button type="primary" @click="submitForm('UpdateForm')">提交</el-button>
        <el-button @click="resetForm('UpdateForm')">重置</el-button>
      </el-form-item>
    </el-form>
  </div>
</template>

<script>
import { Base64 } from 'js-base64';
export default {
  data() {
    let validateOldPass = (rule, value, callback) => {
      if (value === '') {
        callback(new Error('请输入原密码'));
      } else {
        if (value !== Base64.decode(localStorage.getItem("password"))) {
          callback(new Error('原密码错误'));
        }
        callback();
      }
    };
    let validateNewPass = (rule, value, callback) => {
      if (value === '') {
        callback(new Error('请输入新密码'));
      } else {
        if (this.UpdateForm.checkPass !== '') {
          this.$refs.UpdateForm.validateField('checkPass');
        }
        callback();
      }
    };
    let validateCheckPass = (rule, value, callback) => {
      if (value === '') {
        callback(new Error('请再次输入新密码'));
      } else if (value !== this.UpdateForm.newPass) {
        callback(new Error('两次输入密码不一致!'));
      } else {
        callback();
      }
    };
    return {
      UpdateForm: {
        oldPass: '',
        newPass: '',
        checkPass: '',
      },
      rules: {
        oldPass: [
          {validator: validateOldPass, trigger: 'blur'}
        ],
        newPass: [
          {validator: validateNewPass, trigger: 'blur'}
        ],
        checkPass: [
          {validator: validateCheckPass, trigger: 'blur'}
        ],
      },
      UpdateStatus:false
    };
  },
  methods: {
    submitForm(formName) {
      console.log(4)
      this.$refs[formName].validate((valid) => {
        console.log(valid);
        const req = {
          "id": Base64.decode(localStorage.getItem("username")),
          "name": "",
          "password": this.UpdateForm.newPass,
          "phonenumber": "",
        }
        fetch(this.$URL + "/User/Update/Carrier", {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
            "Authorization": "Bearer " + Base64.decode(localStorage.getItem("token"))
          },
          body: JSON.stringify(req),
        }).then((response) => {
          console.log(req);
          let result = response.json();
          result.then((res) => {
                console.log(res);
                this.UpdateStatus = res.status;
                if(this.UpdateStatus===1)
                  localStorage.setItem("password",Base64.encode(this.UpdateForm.newPass));
              }
          )
        })
      });
    },
    resetForm(formName) {
      this.$refs[formName].resetFields();
    }
  },
  mounted() {
    console.log(5);
  }
}
</script>

<style>

</style>
