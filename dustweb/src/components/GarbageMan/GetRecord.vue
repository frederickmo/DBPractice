<template>
  <el-table
      :data="tableData"
      style="width: 100%">
    <el-table-column type="expand">
      <template #default="props">
        <el-form label-position="left" inline class="demo-table-expand">
          <el-form-item label="垃圾编号">
            <span>{{ props.row.gar_id }}</span>
          </el-form-item>
          <el-form-item label="垃圾类型">
            <span>{{ props.row.type }}</span>
          </el-form-item>
<!--          <el-form-item label="检查员">-->
<!--            <span>{{ props.row.id }}</span>-->
<!--          </el-form-item>-->
          <el-form-item label="垃圾桶">
            <span>{{ props.row.dustbin_id }}</span>
          </el-form-item>
          <el-form-item label="运输车">
            <span>{{ props.row.truck_id }}</span>
          </el-form-item>
          <el-form-item label="处理状态">
            <span>{{ props.row.status===0?"已创建":props.row.status===1?"已入桶":props.row.status===2?"运输中":props.row.status===3?"到达处理站":"失败"}}</span>
          </el-form-item>
          <el-form-item label="处理站">
            <span>{{ props.row.plant_name }}</span>
          </el-form-item>
          <el-form-item label="最新时间">
            <span>{{ props.row.latest_time }}</span>
          </el-form-item>
        </el-form>
      </template>
    </el-table-column>
    <el-table-column
        label="垃圾编号"
        prop="gar_id">
    </el-table-column>
    <el-table-column
        label="处理状态">
        <template #default="scope">
          <span v-if="scope.row.status === 0">已创建</span>
          <span v-else-if="scope.row.status === 1">已入桶</span>
          <span v-else-if="scope.row.status === 2">运输中</span>
          <span v-else-if="scope.row.status === 3">到达处理站</span>
          <span v-else>失败</span>
        </template>
    </el-table-column>
    <el-table-column
        label="最新时间"
        prop="latest_time">
    </el-table-column>
  </el-table>
</template>

<script>
import { Base64 } from 'js-base64';
export default {
  data() {

    return {
      ThrowRecord: {
        gar_id: "",
        type: "",
        dustbin_id: "",
        user_id: "",
        truck_id:"",
        status: "",
        latest_time: "",
        plant_name: "",
      },
      showDetail: false,
      tableData: []
    }
  },
  mounted() {
    console.log(1);
    console.log(localStorage.token);
    console.log(Base64.decode(localStorage.username));
    const url = this.$store.state.URL+"/Garbage/GetAll?req=" + Base64.decode(localStorage.username);
    fetch(url, {
      method: "GET",
      headers: {
        accept: "text/plain",
        Authorization: "Bearer " + Base64.decode(localStorage.token),
      },
    }).then((response) => {
      console.log(response);
      let result=response.json();
      result.then((data) => {
        console.log(data);
        this.tableData = data;
      });
    })

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
