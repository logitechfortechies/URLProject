<template>
  <div class="dashboard">
    <SidebarSubsection />
    <ContainerSubsection />
    <DropdownMenuSubsection />
    <div class="dropdown-create-new">
      <div class="nav-item-6">
        <div class="frame-8">
          <img class="vector-52" alt="Vector" :src="vector93" />
          <img class="vector-53" alt="Vector" :src="vector94" />
        </div>
        <div class="text-wrapper-25">Link</div>
        <div class="frame-9">
          <div class="frame-10">
            <img class="vector-54" alt="Vector" :src="vector95" />
          </div>
          <div class="text-wrapper-26">+ L</div>
        </div>
      </div>
      <div class="nav-item-6">
        <img class="frame-8" alt="Frame" :src="frame2" />
        <div class="text-wrapper-25">QR Code</div>
        <div class="frame-9">
          <div class="frame-10">
            <img class="vector-54" alt="Vector" :src="vector96" />
          </div>
          <div class="text-wrapper-26">+ G</div>
        </div>
      </div>
      <div class="nav-item-6">
        <div class="frame-8">
          <img class="vector-55" alt="Vector" :src="vector97" />
          <img class="vector-56" alt="Vector" :src="vector98" />
        </div>
        <div class="text-wrapper-25">Page</div>
        <div class="frame-9">
          <div class="frame-10">
            <img class="vector-54" alt="Vector" :src="vector99" />
          </div>
          <div class="text-wrapper-26">+ P</div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { ContainerSubsection } from "./ContainerSubsection";
import { DropdownMenuSubsection } from "./DropdownMenuSubsection";
import { SidebarSubsection } from "./SidebarSubsection";
import frame2 from "./frame-2.svg";
import vector93 from "./vector-93.svg";
import vector94 from "./vector-94.svg";
import vector95 from "./vector-95.svg";
import vector96 from "./vector-96.svg";
import vector97 from "./vector-97.svg";
import vector98 from "./vector-98.svg";
import vector99 from "./vector-99.svg";

export default {
  name: "Dashboard",
  components: {
    ContainerSubsection,
    DropdownMenuSubsection,
    SidebarSubsection,
  },
  data() {
    return {
      frame2,
      vector93,
      vector94,
      vector95,
      vector96,
      vector97,
      vector98,
      vector99,
    };
  },
};
</script>

<style>
.dashboard {
  align-items: flex-start;
  background-color: var(--gray-blue25);
  border-radius: 14.23px;
  display: flex;
  overflow: hidden;
  position: relative;
  width: 1214.27px;
}

.dashboard .dropdown-create-new {
  align-items: flex-start;
  border: 0.84px solid;
  border-color: var(--gray-blue100);
  border-radius: 6.75px;
  box-shadow: 0px 3.37px 6.75px #1018280d;
  display: flex;
  flex-direction: column;
  left: 691px;
  overflow: hidden;
  position: absolute;
  top: 54px;
  width: 169px;
}

.dashboard .nav-item-6 {
  align-items: center;
  align-self: stretch;
  background-color: #ffffff;
  display: flex;
  flex: 0 0 auto;
  gap: 6.75px;
  padding: 8.43px 11.81px;
  position: relative;
  width: 100%;
}

.dashboard .frame-8 {
  height: 16.86px;
  position: relative;
  width: 16.86px;
}

.dashboard .vector-52 {
  height: 46.02%;
  left: 37.50%;
  position: absolute;
  top: 12.50%;
  width: 41.65%;
}

.dashboard .vector-53 {
  height: 46.02%;
  left: 12.52%;
  position: absolute;
  top: 33.14%;
  width: 41.65%;
}

.dashboard .text-wrapper-25 {
  color: var(--gray-blue600);
  flex: 1;
  font-family: "Manrope-SemiBold", Helvetica;
  font-size: 11.8px;
  font-weight: 600;
  letter-spacing: 0;
  line-height: normal;
  margin-top: -0.41px;
  position: relative;
}

.dashboard .frame-9 {
  align-items: center;
  background-color: #ffffff;
  border: 0.84px solid;
  border-color: var(--gray-blue50);
  border-radius: 3.37px;
  display: inline-flex;
  flex: 0 0 auto;
  gap: 2.53px;
  justify-content: center;
  padding: 1.69px 3.37px;
  position: relative;
}

.dashboard .frame-10 {
  height: 10.12px;
  position: relative;
  width: 10.12px;
}

.dashboard .vector-54 {
  height: 58.33%;
  left: 16.67%;
  position: absolute;
  top: 16.67%;
  width: 58.33%;
}

.dashboard .text-wrapper-26 {
  color: var(--gray-blue300);
  font-family: "Manrope-SemiBold", Helvetica;
  font-size: 8.4px;
  font-weight: 600;
  letter-spacing: 0;
  line-height: normal;
  margin-top: -0.84px;
  position: relative;
  width: fit-content;
}

.dashboard .vector-55 {
  height: 66.67%;
  left: 12.50%;
  position: absolute;
  top: 12.50%;
  width: 66.67%;
}

.dashboard .vector-56 {
  height: 0;
  left: 12.50%;
  position: absolute;
  top: 33.33%;
  width: 66.67%;
}
</style>
