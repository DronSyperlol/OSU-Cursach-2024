import './Loading.css'
import Lottie from 'react-lottie';
import animationData from './loading.json';

export function Loading({height, width}) {
    const defaultOptions = {
        loop: true,
        autoplay: true,
        animationData: animationData,
        rendererSettings: {
          preserveAspectRatio: "xMidYMid slice"
        }
      };
    
    return (
        <Lottie 
          options={defaultOptions}
          height={height ? height : 100}
          width={width ? width : 100}
        />
    );
}