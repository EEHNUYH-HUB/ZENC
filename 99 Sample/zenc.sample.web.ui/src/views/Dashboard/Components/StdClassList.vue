<template>
  <div class="card">
    <div class="card-header p-3">
      <h6 class="mb-0">수강 리스트</h6>
      <div class="d-flex">
        <div class="mb-0 text-sm p font-weight-bold widget-calendar-day">
          수강중인 과목을 볼수 있습니다.
        </div>

      </div>
    </div>
    <div class="card-body p-3 pt-0">
      <StdLectureList>
      </StdLectureList>
    </div>
  </div>
</template>

<script setup>
import StdLectureList from '@/views/LectureList/Components/StdLectureList.vue';
</script>
