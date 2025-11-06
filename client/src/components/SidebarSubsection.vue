<template>
  <div class="sidebar-subsection">
    <img class="logo-wrapper" alt="Logo wrapper" :src="logoWrapper" />

    <div class="menu-wrapper">
      <div class="nav-item">
        <div class="frame">
          <img class="vector" alt="Vector" :src="vector100" />
          <img class="img" alt="Vector" :src="vector101" />
        </div>
        <div class="text-wrapper">Dashboard</div>
      </div>

      <div class="div">
        <div class="frame">
          <img class="vector-2" alt="Vector" :src="vector" />
          <img class="vector-3" alt="Vector" :src="image" />
        </div>
        <div class="text-wrapper-2">Links</div>
        <div class="vector-wrapper">
          <img class="vector-4" alt="Vector" :src="vector2" />
        </div>
      </div>

      <div class="div">
        <img class="frame" alt="Frame" :src="frame" />
        <div class="text-wrapper-2">QR Codes</div>
        <div class="vector-wrapper">
          <img class="vector-4" alt="Vector" :src="vector3" />
        </div>
      </div>

      <div class="div">
        <div class="frame">
          <img class="vector-5" alt="Vector" :src="vector4" />
          <img class="vector-6" alt="Vector" :src="vector5" />
        </div>
        <div class="text-wrapper-2">Pages</div>
        <div class="vector-wrapper">
          <img class="vector-4" alt="Vector" :src="vector6" />
        </div>
      </div>

      <div class="div">
        <div class="frame">
          <img class="vector-7" alt="Vector" :src="vector7" />
          <img class="vector-8" alt="Vector" :src="vector8" />
        </div>
        <div class="text-wrapper-2">Analytics</div>
      </div>

      <div class="nav-item-2">
        <div class="frame">
          <img class="vector-9" alt="Vector" :src="vector9" />
          <img class="vector-10" alt="Vector" :src="vector10" />
          <img class="vector-11" alt="Vector" :src="vector11" />
          <img class="vector-12" alt="Vector" :src="vector12" />
          <img class="vector-13" alt="Vector" :src="vector13" />
        </div>
        <div class="text-wrapper-2">Domains</div>
        <div class="upgrade-button">
          <div class="text-wrapper-3">Upgrade</div>
        </div>
      </div>
    </div>

    <div class="menu-wrapper">
      <div class="div">
        <div class="frame">
          <img class="vector-14" alt="Vector" :src="vector14" />
          <img class="vector-9" alt="Vector" :src="vector15" />
          <img class="vector-15" alt="Vector" :src="vector16" />
          <img class="vector-16" alt="Vector" :src="vector17" />
          <img class="vector-17" alt="Vector" :src="vector18" />
          <img class="vector-18" alt="Vector" :src="vector19" />
        </div>
        <div class="text-wrapper-2">Support</div>
      </div>

      <div class="div">
        <div class="frame">
          <img class="vector-9" alt="Vector" :src="vector20" />
          <img class="vector-19" alt="Vector" :src="vector21" />
        </div>
        <div class="text-wrapper-2">Settings</div>
      </div>
    </div>

    <div class="sidebar-bottom">
      <div class="card">
        <div class="text-wrapper-4">2/10 links available</div>
        <p class="p">
         
        </p>
        <div class="progress-bar">
          <img class="line" alt="Line" :src="line1" />
          <img class="line-2" alt="Line" :src="line2" />
        </div>
        <button class="button">
          <div class="text-wrapper-5">Upgrade now</div>
          <div class="frame">
            <img class="vector-20" alt="Vector" :src="vector22" />
            <img class="vector-21" alt="Vector" :src="vector23" />
            <img class="vector-22" alt="Vector" :src="vector24" />
          </div>
        </button>
        <div class="frame-2">
          <img class="vector-23" alt="Vector" :src="vector25" />
          <img class="vector-23" alt="Vector" :src="vector26" />
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import frame from "./frame.svg";
import image from "./image.svg";
import line1 from "./line-1.svg";
import line2 from "./line-2.svg";
import logoWrapper from "./logo-wrapper.svg";
import vector2 from "./vector-2.svg";
import vector3 from "./vector-3.svg";
import vector4 from "./vector-4.svg";
import vector5 from "./vector-5.svg";
import vector6 from "./vector-6.svg";
import vector7 from "./vector-7.svg";
import vector8 from "./vector-8.svg";
import vector9 from "./vector-9.svg";
import vector10 from "./vector-10.svg";
import vector11 from "./vector-11.svg";
import vector12 from "./vector-12.svg";
import vector13 from "./vector-13.svg";
import vector14 from "./vector-14.svg";
import vector15 from "./vector-15.svg";
import vector16 from "./vector-16.svg";
import vector17 from "./vector-17.svg";
import vector18 from "./vector-18.svg";
import vector19 from "./vector-19.svg";
import vector20 from "./vector-20.svg";
import vector21 from "./vector-21.svg";
import vector22 from "./vector-22.svg";
import vector23 from "./vector-23.svg";
import vector24 from "./vector-24.svg";
import vector25 from "./vector-25.svg";
import vector26 from "./vector-26.svg";
import vector100 from "./vector-100.svg";
import vector101 from "./vector-101.svg";
import vector from "./vector.svg";

export default {
  name: "SidebarSubsection",
  data() {
    return {
      logoWrapper,
      frame,
      image,
      line1,
      line2,
      vector,
      vector100,
      vector101,
      vector2,
      vector3,
      vector4,
      vector5,
      vector6,
      vector7,
      vector8,
      vector9,
      vector10,
      vector11,
      vector12,
      vector13,
      vector14,
      vector15,
      vector16,
      vector17,
      vector18,
      vector19,
      vector20,
      vector21,
      vector22,
      vector23,
      vector24,
      vector25,
      vector26,
    };
  },
};
</script>

<style>
.sidebar-subsection {
  /* Add your styles here */
}

.logo-wrapper {
  /* Add your styles here */
}

.menu-wrapper {
  /* Add your styles here */
}

.nav-item {
  /* Add your styles here */
}

.frame {
  /* Add your styles here */
}

.text-wrapper {
  /* Add your styles here */
}

.div {
  /* Add your styles here */
}

.text-wrapper-2 {
  /* Add your styles here */
}

.vector-wrapper {
  /* Add your styles here */
}

.sidebar-bottom {
  /* Add your styles here */
}

.card {
  /* Add your styles here */
}

.text-wrapper-4 {
  /* Add your styles here */
}

.p {
  /* Add your styles here */
}

.progress-bar {
  /* Add your styles here */
}

.button {
  /* Add your styles here */
}

.text-wrapper-5 {
  /* Add your styles here */
}

.frame-2 {
  /* Add your styles here */
}
</style>
