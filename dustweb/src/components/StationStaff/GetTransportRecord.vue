<template>
  <el-table
      :data="tableData"
      style="width: 100%"
      border>
    <el-table-column type="expand">
      <template #default="props" >
        <el-form label-position="left" inline class="demo-table-expand">
          <el-form-item label="运输编号">
            <span>{{ props.row.trans_id }}</span>
          </el-form-item>
          <el-form-item label="垃圾桶">
            <span>{{ props.row.dustbin_id }}</span>
          </el-form-item>
          <el-form-item label="运输车辆">
            <span>{{ props.row.truck_id }}</span>
          </el-form-item>
          <el-form-item label="运输状态">
            <span v-if="props.row.start_time!=props.row.end_time">已结束</span>
            <span v-else>运输中</span>
          </el-form-item>
          <el-form-item label="运输终点">
            <span>{{ props.row.plant_name }}</span>
          </el-form-item>
          <el-form-item label="开始时间">
            <span>{{ props.row.start_time }}</span>
          </el-form-item>
          <el-form-item label="结束时间">
            <span v-if="props.row.start_time!=props.row.end_time">{{ props.row.end_time }}</span>
          </el-form-item>
        </el-form>
      </template>
    </el-table-column>
    <el-table-column
        label="运输编号"
        prop="trans_id">
    </el-table-column>
    <el-table-column
        label="开始时间"
        prop="start_time">
    </el-table-column>
    <el-table-column
        label="运输状态">
      <template #default="scope">
        <span v-if="scope.row.end_time === scope.row.start_time">运输中</span>
        <span v-else>已结束</span>
      </template>
    </el-table-column>
    <el-table-column
        fixed="right"
        label="操作"
        width="100">
      <template #default="scope">
        <el-button :disabled="scope.row.end_time != scope.row.start_time" @click="TransportEnd(scope.row.trans_id)" type="text" >到站</el-button>
      </template>
    </el-table-column>
  </el-table>
</template>

<script>
import {Base64} from 'js-base64';

export default {
  data() {
    return {
      UserInfo: {
        account: Base64.decode(localStorage.getItem("username")),
        name: '',
        phone: '',
        plant_name:'',
        password: Base64.decode(localStorage.getItem("password")),
      },
      TransportRecord: {
        "trans_id": "3952108210721191201",
        "dustbin_id": "D002",
        "truck_id": "T01",
        "carrier_id": "3952108",
        "plant_name": "",
        "start_time": "2021-07-21T19:12:01",
        "end_time": "2021-07-21T19:12:01"
      },
      showDetail: false,
      tableData: []
    }
  },
  methods:{
    getUserInfo() {
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
          this.UserInfo.plant_name=result.plantname;
          this.getData();
        })
      })
    },
    getData(){
      console.log(1,this.UserInfo.plant_name);
      const url = this.$URL + "/Transport/StaffGet?req=" + this.UserInfo.plant_name;
      fetch(url, {
        method: "GET",
        headers: {
          accept: "text/plain",
          Authorization: "Bearer " + Base64.decode(localStorage.token),
        },
      }).then((response) => {
        console.log(response);
        let result = response.json();
        result.then((data) => {
          console.log(data);
          this.tableData = data;
        });
      })
    },
    TransportEnd(elm){
      fetch(this.$URL + "/Transport/TransportEnd?req=" + elm, {
        method: "GET",
        headers: {
          "Authorization": "Bearer " + Base64.decode(localStorage.getItem("token")),
        },
      }).then((response) => {
        let result = response.json();
        result.then((result) => {
          if(result.status===1)
            this.getData();
        })
      })
    }
  },
  mounted() {
    console.log(1);
    console.log(localStorage.token);
    console.log(Base64.decode(localStorage.username));
    this.getUserInfo();
  }
}
</script>

<style>
.demo-table-expand {
  font-size: 0;
}

.demo-table-expand label {
  width: 90px;
  color: #99a9bf;
}

.demo-table-expand .el-form-item {
  margin-right: 0;
  margin-bottom: 0;
  width: 50%;
}
</style>
