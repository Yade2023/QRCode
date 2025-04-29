<template>
  <div class="admin-container">
    <el-card class="qr-card">
      <template #header>
        <div class="card-header">
          <h2>QR Code 產生器</h2>
        </div>
      </template>
      
      <div class="qr-content">
        <!-- 管理員工號輸入表單 -->
        <el-form 
          :model="formData"
          :rules="rules"
          ref="formRef"
          label-width="100px"
          class="generate-form"
        >
          <el-form-item label="管理員工號" prop="employeeId">
            <el-input 
              v-model="formData.employeeId"
              placeholder="請輸入管理員工號"
              :disabled="loading"
            ></el-input>
          </el-form-item>
        </el-form>

        <!-- QR Code 顯示區域 -->
        <div v-if="qrCodeData" class="qr-display-container">
          <div class="qr-display">
            <qrcode-vue :value="qrCodeData" :size="200" level="H"></qrcode-vue>
          </div>
          
          <!-- 倒計時顯示 -->
          <div class="countdown-timer" :class="{ 'warning': timeLeft <= 60 }">
            剩餘時間: {{ formatTime(timeLeft) }}
          </div>
        </div>
        
        <div class="button-group">
          <el-button type="primary" @click="generateQRCode" :loading="loading">
            產生 QR Code
          </el-button>
          <el-button type="success" @click="downloadQRCode" :disabled="!qrCodeData">
            下載 QR Code
          </el-button>
        </div>
      </div>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, onUnmounted } from 'vue'
import QrcodeVue from 'qrcode.vue'
import axios from 'axios'
import { ElMessage } from 'element-plus'
import type { FormInstance, FormRules } from 'element-plus'

// 定義響應式變數
const qrCodeData = ref('')
const loading = ref(false)
const formRef = ref<FormInstance>()
const timeLeft = ref(0)

let countdownTimer: ReturnType<typeof setTimeout> | null = null;
// 表單數據
const formData = ref({
  employeeId: ''
})

// 表單驗證規則
const rules: FormRules = {
  employeeId: [
    { required: true, message: '請輸入管理員工號', trigger: 'blur' },
    { min: 1, max: 20, message: '長度需在 1 到 20 個字元之間', trigger: 'blur' }
  ]
}

// 格式化時間顯示（將秒數轉換為 分:秒 格式）
const formatTime = (seconds: number) => {
  const minutes = Math.floor(seconds / 60)
  const remainingSeconds = seconds % 60
  return `${minutes}:${remainingSeconds.toString().padStart(2, '0')}`
}

// 開始倒計時
const startCountdown = (expirationTime: number) => {
  // 清除之前的計時器
  if (countdownTimer) {
    clearInterval(countdownTimer)
  }

  timeLeft.value = expirationTime

  countdownTimer = setInterval(() => {
    if (timeLeft.value > 0) {
      timeLeft.value--
    } else {
      // QR Code 過期
      qrCodeData.value = ''
      ElMessage.warning('QR Code 已過期')
      if (countdownTimer) {
        clearInterval(countdownTimer)
      }
    }
  }, 1000)
}

// 生成 QR Code
const generateQRCode = async () => {
  try {
    // 表單驗證
    if (!formRef.value) return;
    await formRef.value.validate();

    loading.value = true
    const response = await axios.post('http://localhost:5198/api/QRCode/generate', {
      employeeId: formData.value.employeeId
    }, {
      headers: {
        'Content-Type': 'application/json',
        'Accept': 'application/json'
      }
    });

    console.log('Response:', response);

    if (response.data && response.data.qrCodeData) {
      qrCodeData.value = response.data.qrCodeData;
      ElMessage.success('QR Code 產生成功！')
      startCountdown(900) // 300 秒 = 5 分鐘
    } else {
      ElMessage.error(response.data.message || 'QR Code 生成失敗')
    }
  } catch (error: any) {
    console.error('Error generating QR code:', error)
    console.error('Error details:', {
      message: error.message,
      response: error.response,
      request: error.request
    });
    ElMessage.error(`產生 QR Code 時發生錯誤: ${error.message || '未知錯誤'}`);  
  } finally {
    loading.value = false;
  }
};

// 下載 QR Code
const downloadQRCode = () => {
  const canvas = document.querySelector('.qr-display canvas') as HTMLCanvasElement
  if (!canvas) {
    ElMessage.warning('QR Code 尚未產生')
    return
  }

  const link = document.createElement('a')
  link.download = `qrcode-${Date.now()}.png`
  link.href = canvas.toDataURL('image/png')
  link.click()
  ElMessage.success('QR Code 下載成功！')
}

// 組件卸載時清理計時器
onUnmounted(() => {
  if (countdownTimer) {
    clearInterval(countdownTimer)
  }
})
</script>

<style scoped>
.admin-container {
  max-width: 800px;
  margin: 2rem auto;
  padding: 0 1rem;
}

.qr-card {
  width: 100%;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.qr-content {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 2rem;
  padding: 1rem;
}

.generate-form {
  width: 100%;
  max-width: 400px;
}

.qr-display-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 1rem;
}

.qr-display {
  background: white;
  padding: 1rem;
  border-radius: 8px;
  box-shadow: 0 2px 12px 0 rgba(0, 0, 0, 0.1);
}

.countdown-timer {
  font-size: 1.2rem;
  font-weight: bold;
  color: #409EFF;
}

.countdown-timer.warning {
  color: #E6A23C;
}

.button-group {
  display: flex;
  gap: 1rem;
}
</style>