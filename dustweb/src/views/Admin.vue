<template>
  <el-switch
    v-model="value"
    active-color="#13ce66"
    inactive-color="#ff4949"
  ></el-switch>
  {{value}}
</template>

<script>
export default {
  data() {
    return {
      value: true,
    };
  },
};
</script>

<style>
</style>