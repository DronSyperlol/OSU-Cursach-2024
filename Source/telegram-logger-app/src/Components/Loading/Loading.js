import './Loading.css'
import Lottie from 'react-lottie';
import loadingLottie from './loading.json';

export function Loading({height, width}) {
    const defaultOptions = {
        loop: true,
        autoplay: true,
        animationData: loadingLottie,
        rendererSettings: {
          preserveAspectRatio: "xMidYMid slice"
        }
      };
    
    return (
      <div>
        <Lottie 
          options={defaultOptions}
          height={height ? height : 100}
          width={width ? width : 100}
        />
      </div>
    );
}