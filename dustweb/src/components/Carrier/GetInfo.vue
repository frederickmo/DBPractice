<template>
  <el-dialog title="修改头像" v-model="updateAvatarFormVisible">
    <div style="margin-bottom: 10px">选择头像上传（JPG格式，大小不超过1M）</div>
    <el-upload
      class="avatar-uploader"
      action=""
      :show-file-list="false"
      :on-success="handleAvatarSuccess"
      :before-upload="beforeAvatarUpload"
      >
      <img v-if="imageUrl" :src="imageUrl" class="avatar">
      <i v-else class="el-icon-plus avatar-uploader-icon"></i>
    </el-upload>  
  </el-dialog>
  <div>
    <el-row class="row-bg" justify="left">
      <el-col :span="3" :offset="2">
        <div class="grid-content bg-purple">头像</div>
      </el-col>
      <el-col :span="3">
        <el-button circle style="padding: 0 0 0 0;" @click="updateAvatarFormVisible=true">
          <el-avatar shape="circle" :size="60" fit="cover" src="https://636c-cloud1-6gti323d7294495c-1306017304.tcb.qcloud.la/other_resources/D073A6D3-5297-4F64-9D73-4EB44308FF48_1_105_c.jpeg?sign=bfa532f94c3d03eeb54d1badf91775f8&t=1627453440"></el-avatar>
        </el-button>
      </el-col>
    </el-row>
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
        <div class="grid-content bg-purple">当前车辆</div>
      </el-col>
      <el-col :span="3">
        <div class="grid-content bg-purple-light">{{ UserInfo.Truck }} 垃圾车</div>
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
        Truck: Base64.decode(localStorage.getItem("Truck")),
        phone: '',
        address: '',
        password: Base64.decode(localStorage.getItem("password")),
      },
      updateAvatarFormVisible: false,
      imageUrl: ''
    };
  },
  mounted() {
    console.log(this.UserInfo.account, this.UserInfo.password);
    fetch(this.$URL + "/User/GetInformation/Carrier?req=" + this.UserInfo.account, {
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
      })
    })
  },
  methods: {
    handleAvatarSuccess(res, file) {
      this.imageUrl = URL.createObjectURL(file.raw);
    },
    createAvatarUpload(file) {
      const isJPG = file.type === 'image/jepg';
      const isLt2M = file.size / 1024 / 1024 < 1;
      if (!isJPG) {
        this.$message.error('上传头像只能是JPG格式');
      }
      if (!isLt2M) {
        this.$message.error('上传头像图片大小不能超过1MB')
      }
      return isJPG && isLt2M;
    }
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

.avatar-uploader .el-upload {
  border: 1px dashed #d9d9d9;
  border-radius: 6px;
  cursor: pointer;
  position: relative;
  overflow: hidden;
}
.avatar-uploader .el-upload:hover {
  border-color: #409EFF;
}
.avatar-uploader-icon {
  font-size: 28px;
  color: #8c939d;
  width: 178px;
  height: 178px;
  line-height: 178px;
  text-align: center;
}
.avatar {
  width: 178px;
  height: 178px;
  display: block;
}
</style>