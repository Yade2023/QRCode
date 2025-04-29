<template>
  <div class="checkin-container">
    <el-card class="checkin-card">
      <template #header>
        <div class="card-header">
          <h2>員工打卡</h2>
        </div>
      </template>
      
      <div class="checkin-content">
        <el-form 
          :model="formData"
          :rules="rules"
          ref="formRef"
          label-width="100px"
          class="checkin-form"
        >
          <el-form-item label="員工編號" prop="employeeId">
            <el-input 
              v-model="formData.employeeId"
              placeholder="請輸入員工編號"
              :disabled="loading"
            ></el-input>
          </el-form-item>

          <el-form-item>
            <el-button 
              type="primary" 
              @click="submitCheckIn"
              :loading="loading"
            >
              打卡
            </el-button>
          </el-form-item>
        </el-form>
      </div>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import type { FormInstance, FormRules } from 'element-plus'
import { ElMessage } from 'element-plus'
import axios from 'axios'

const route = useRoute()
const router = useRouter()
const formRef = ref<FormInstance>()
const loading = ref(false)

const formData = ref({
  employeeId: '',
  qrCodeId: '',
  timestamp: ''
})

const rules: FormRules = {
  employeeId: [
    { required: true, message: '請輸入員工編號', trigger: 'blur' },
    { min: 1, max: 20, message: '長度需在 1 到 20 個字元之間', trigger: 'blur' }
  ]
}

onMounted(() => {
  // 從 URL 參數獲取 QR Code ID 和時間戳
  const { id, t } = route.query
  
  if (!id || !t) {
    ElMessage.error('無效的 QR Code')
    router.push('/admin')
    return
  }

  formData.value.qrCodeId = id as string
  formData.value.timestamp = t as string
})

const submitCheckIn = async () => {
  if (!formRef.value) return

  try {
    await formRef.value.validate()
    
    loading.value = true
    const response = await axios.post('http://localhost:5198/api/QRCode/checkin', {
      "EmployeeNumber": formData.value.employeeId,
      "QRCodeId": formData.value.qrCodeId,
      "Timestamp": formData.value.timestamp
    })

    if (response.data.success) {
      ElMessage.success('打卡成功！')
      // 可以根據需求決定是否要跳轉到其他頁面
      // router.push('/success')
    } else {
      console.log('ElMessage:', response);
      ElMessage.error(response.data.message || '打卡失敗')
      
    }
  } catch (error: any) {
    if (error.response?.data?.message) {
      ElMessage.error(error.response.data.message)
    } else if (error.message) {
      ElMessage.error(error.message)
    } else {
      console.log('ElMessage:', error);
      ElMessage.error('打卡時發生錯誤')
    }
    console.error('Check-in error:', error)
  } finally {
    loading.value = false
  }
}
</script>

<style scoped>
.checkin-container {
  max-width: 600px;
  margin: 2rem auto;
  padding: 0 1rem;
}

.checkin-card {
  width: 100%;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.checkin-content {
  padding: 1rem;
}

.checkin-form {
  max-width: 400px;
  margin: 0 auto;
}
</style>